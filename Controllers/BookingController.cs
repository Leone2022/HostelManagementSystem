using HostelMS.Models;
using HostelMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HostelMS.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookingController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Booking
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Index()
        {
            // Show only private hostels that are active
            var privateHostels = await _context.Hostels
                .Where(h => h.IsActive && h.ManagementType == ManagementType.PrivatelyManaged)
                .OrderBy(h => h.Name)
                .ToListAsync();

            return View(privateHostels);
        }

        // GET: Booking/Create/5 (5 is hostelId)
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Create(int id)
        {
            var hostel = await _context.Hostels
                .Include(h => h.Rooms.Where(r => r.Status == RoomStatus.Available || r.Status == RoomStatus.PartiallyAssigned))
                .FirstOrDefaultAsync(h => h.HostelId == id);

            if (hostel == null)
            {
                return NotFound();
            }

            // Check if hostel has available rooms
            if (!hostel.Rooms.Any())
            {
                TempData["ErrorMessage"] = "Sorry, there are no available rooms in this hostel.";
                return RedirectToAction(nameof(Index));
            }

            // Get current student
            var student = await _userManager.GetUserAsync(User);
            if (student == null)
            {
                return NotFound();
            }

            // Check if student already has a pending or approved booking
            var existingBooking = await _context.Bookings
                .AnyAsync(b => b.UserId == student.Id && 
                              (b.Status == BookingStatus.Pending || 
                               b.Status == BookingStatus.Approved || 
                               b.Status == BookingStatus.CheckedIn));

            if (existingBooking)
            {
                TempData["ErrorMessage"] = "You already have an active booking. Please check your booking history.";
                return RedirectToAction(nameof(MyBookings));
            }

            // Prepare room options for the hostel
            var roomList = hostel.Rooms
                .Select(r => new SelectListItem
                {
                    Value = r.RoomId.ToString(),
                    Text = $"Room {r.RoomNumber} - {r.Type} - UGX {r.PricePerSemester:N0}/semester - {r.Capacity - r.CurrentOccupancy} bed(s) available"
                })
                .ToList();

            var model = new BookingCreateViewModel
            {
                HostelId = hostel.HostelId,
                HostelName = hostel.Name,
                AvailableRooms = roomList,
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddMonths(4), // Default to one semester
                UserId = student.Id
            };

            return View(model);
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get selected room
                var room = await _context.Rooms
                    .Include(r => r.Hostel)
                    .FirstOrDefaultAsync(r => r.RoomId == model.RoomId);

                if (room == null)
                {
                    TempData["ErrorMessage"] = "The selected room does not exist.";
                    return RedirectToAction(nameof(Index));
                }

                // Check if room is still available
                if (room.Status == RoomStatus.FullyAssigned || room.Status == RoomStatus.UnderMaintenance)
                {
                    TempData["ErrorMessage"] = "This room is no longer available for booking.";
                    return RedirectToAction(nameof(Create), new { id = model.HostelId });
                }

                // Create new booking
                var booking = new Booking
                {
                    UserId = model.UserId,
                    RoomId = model.RoomId,
                    BookingDate = DateTime.Now,
                    CheckInDate = model.CheckInDate,
                    CheckOutDate = model.CheckOutDate,
                    TotalAmount = room.PricePerSemester,
                    Status = BookingStatus.Pending,
                    Comments = model.Comments ?? string.Empty
                };

                _context.Add(booking);
                await _context.SaveChangesAsync();

                // Create payment record
                var payment = new Payment
                {
                    BookingId = booking.BookingId,
                    Amount = model.Amount,
                    PaymentDate = DateTime.Now,
                    Method = model.PaymentMethod,
                    Status = PaymentStatus.Pending,
                    TransactionReference = model.TransactionReference,
                    Notes = $"Payment for {room.Hostel?.Name ?? "Unknown Hostel"}, Room {room.RoomNumber}"
                };

                // Handle payment proof upload
                if (model.PaymentProof != null)
                {
                    string uniqueFileName = $"{Guid.NewGuid()}_{model.PaymentProof.FileName}";
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "payments");
                    
                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.PaymentProof.CopyToAsync(fileStream);
                    }

                    payment.ProofOfPaymentUrl = $"/uploads/payments/{uniqueFileName}";
                }

                _context.Add(payment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your booking request has been submitted successfully! You will be notified once it's approved.";
                return RedirectToAction(nameof(MyBookings));
            }

            // If we got this far, something failed, reload the form
            // Reload room list
            var hostel = await _context.Hostels
                .Include(h => h.Rooms.Where(r => r.Status == RoomStatus.Available || r.Status == RoomStatus.PartiallyAssigned))
                .FirstOrDefaultAsync(h => h.HostelId == model.HostelId);

            if (hostel != null)
            {
                model.HostelName = hostel.Name;
                model.AvailableRooms = hostel.Rooms
                    .Select(r => new SelectListItem
                    {
                        Value = r.RoomId.ToString(),
                        Text = $"Room {r.RoomNumber} - {r.Type} - UGX {r.PricePerSemester:N0}/semester - {r.Capacity - r.CurrentOccupancy} bed(s) available"
                    })
                    .ToList();
            }

            return View(model);
        }

        // GET: Booking/MyBookings
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyBookings()
        {
            var student = await _userManager.GetUserAsync(User);
            if (student == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r != null ? r.Hostel : null)
                .Where(b => b.UserId == student.Id)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Booking/ManageBookings
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> ManageBookings(string status = "Pending", string? searchString = null, int? hostelId = null, DateTime? dateFilter = null)
        {
            var query = _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Room)
                .ThenInclude(r => r != null ? r.Hostel : null)
                .AsQueryable();

            // Filter by status
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<BookingStatus>(status, out var bookingStatus))
            {
                query = query.Where(b => b.Status == bookingStatus);
            }

            // Filter by search string
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(b => 
                    (b.Student != null && (
                        b.Student.FirstName.Contains(searchString) || 
                        b.Student.LastName.Contains(searchString) || 
                        (b.Student.StudentId != null && b.Student.StudentId.Contains(searchString))
                    )) || 
                    (b.Room != null && b.Room.RoomNumber.Contains(searchString)));
            }

            // Filter by hostel
            if (hostelId.HasValue)
            {
                query = query.Where(b => b.Room != null && b.Room.HostelId == hostelId);
            }

            // Filter by date
            if (dateFilter.HasValue)
            {
                DateTime filterDate = dateFilter.Value.Date;
                query = query.Where(b => b.BookingDate.Date == filterDate || 
                                        b.CheckInDate.Date == filterDate || 
                                        b.CheckOutDate.Date == filterDate);
            }

            // Get hostels for filter dropdown
            var hostels = await _context.Hostels
                .OrderBy(h => h.Name)
                .ToListAsync();

            // Count pending bookings
            var pendingCount = await _context.Bookings
                .CountAsync(b => b.Status == BookingStatus.Pending);

            ViewBag.Hostels = hostels;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentFilter = searchString;
            ViewBag.SelectedHostel = hostelId;
            ViewBag.DateFilter = dateFilter?.ToString("yyyy-MM-dd");
            ViewBag.PendingCount = pendingCount;

            var bookings = await query
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Booking/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Room)
                .ThenInclude(r => r != null ? r.Hostel : null)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Check permissions - only allow admins, wardens, or the student who made the booking
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            var isWarden = await _userManager.IsInRoleAsync(currentUser, "Warden");

            if (!isAdmin && !isWarden && booking.UserId != currentUser.Id)
            {
                return Forbid();
            }

            // Get payment information
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == booking.BookingId);

            ViewBag.Payment = payment;

            return View(booking);
        }

        // GET: Booking/ProcessBooking/5
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> ProcessBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Room)
                .ThenInclude(r => r != null ? r.Hostel : null)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Only allow processing of pending bookings
            if (booking.Status != BookingStatus.Pending)
            {
                TempData["ErrorMessage"] = "Only pending bookings can be processed.";
                return RedirectToAction(nameof(ManageBookings));
            }

            // Get payment information
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == booking.BookingId);

            var model = new BookingProcessViewModel
            {
                BookingId = booking.BookingId,
                Booking = booking,
                Payment = payment,
                IsApproved = false
            };

            return View(model);
        }

        // POST: Booking/ProcessBooking
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> ProcessBooking(int BookingId, bool IsApproved, string? RejectionReason)
        {
            // Find the booking
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == BookingId);

            if (booking == null)
            {
                return NotFound();
            }

            // Get current user (admin/warden) who is processing this
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }

            if (IsApproved)
            {
                // Get room details
                var room = booking.Room;
                if (room == null)
                {
                    TempData["ErrorMessage"] = "Room information not available.";
                    return RedirectToAction(nameof(ProcessBooking), new { id = BookingId });
                }

                // Check if this is a single room that already has an approved booking
                if (room.Type == RoomType.Single)
                {
                    // Check if the room already has an approved or checked-in booking
                    var existingApprovals = await _context.Bookings
                        .CountAsync(b => b.RoomId == room.RoomId && 
                                        (b.Status == BookingStatus.Approved || b.Status == BookingStatus.CheckedIn));

                    if (existingApprovals > 0)
                    {
                        TempData["ErrorMessage"] = "Cannot approve booking. This is a single room that already has an approved booking.";
                        return RedirectToAction(nameof(ProcessBooking), new { id = BookingId });
                    }
                }

                // Check if the room would be overbooked with this approval
                if (room.CurrentOccupancy >= room.Capacity)
                {
                    TempData["ErrorMessage"] = "Cannot approve booking. The room is already fully occupied.";
                    return RedirectToAction(nameof(ProcessBooking), new { id = BookingId });
                }

                // Check if there are pending bookings for same room with earlier dates
                // This implements "first come, first served" for pending bookings
                var earlierPendingBookings = await _context.Bookings
                    .AnyAsync(b => b.RoomId == room.RoomId && 
                                b.Status == BookingStatus.Pending && 
                                b.BookingDate < booking.BookingDate);

                if (earlierPendingBookings)
                {
                    TempData["WarningMessage"] = "Note: There are earlier pending booking requests for this room. Consider processing those first.";
                }

                // Approve the booking
                booking.Status = BookingStatus.Approved;
                booking.ApprovedBy = currentUser.Id;
                booking.ApprovalDate = DateTime.Now;
                
                // Update payment status
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.BookingId == booking.BookingId);
                
                if (payment != null)
                {
                    payment.Status = PaymentStatus.Completed;
                    payment.Notes = $"{payment.Notes ?? ""}\nVerified by {currentUser.FirstName} {currentUser.LastName} on {DateTime.Now}";
                }

                TempData["SuccessMessage"] = "Booking has been approved successfully.";
            }
            else
            {
                // Validate rejection reason
                if (string.IsNullOrWhiteSpace(RejectionReason))
                {
                    TempData["ErrorMessage"] = "Rejection reason is required when rejecting a booking.";
                    return RedirectToAction(nameof(ProcessBooking), new { id = BookingId });
                }

                // Reject the booking
                booking.Status = BookingStatus.Rejected;
                booking.RejectionReason = RejectionReason;
                
                // Update payment status
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.BookingId == booking.BookingId);
                
                if (payment != null)
                {
                    payment.Status = PaymentStatus.Failed;
                    payment.Notes = $"{payment.Notes ?? ""}\nRejected by {currentUser.FirstName} {currentUser.LastName}: {RejectionReason}";
                }

                TempData["InfoMessage"] = "Booking has been rejected.";
            }

            // Save changes
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(ManageBookings));
        }

        // POST: Booking/CheckIn/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> CheckIn(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Student)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Only allow check-in for approved bookings
            if (booking.Status != BookingStatus.Approved)
            {
                TempData["ErrorMessage"] = "Only approved bookings can be checked in.";
                return RedirectToAction(nameof(Details), new { id = id });
            }

            // Check if the room is a single room and already has someone checked in
            var room = booking.Room;
            if (room != null && room.Type == RoomType.Single)
            {
                var existingCheckIns = await _context.Bookings
                    .CountAsync(b => b.RoomId == room.RoomId && 
                                   b.Status == BookingStatus.CheckedIn &&
                                   b.BookingId != id);

                if (existingCheckIns > 0)
                {
                    TempData["ErrorMessage"] = "Cannot check in student. This is a single room that already has a student checked in.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }
            }

            // Ensure the room won't be overbooked
            if (room != null && room.CurrentOccupancy >= room.Capacity)
            {
                TempData["ErrorMessage"] = "Cannot check in student. The room is already at full capacity.";
                return RedirectToAction(nameof(Details), new { id = id });
            }

            booking.Status = BookingStatus.CheckedIn;

            // Update the room capacity and student information
            var student = booking.Student;

            if (room != null && student != null)
            {
                // Update room status and occupancy
                room.CurrentOccupancy += 1;
                
                if (room.CurrentOccupancy >= room.Capacity)
                {
                    room.Status = RoomStatus.FullyAssigned;
                }
                else if (room.CurrentOccupancy > 0)
                {
                    room.Status = RoomStatus.PartiallyAssigned;
                }

                // Update student record - handle nullable properties properly
                student.CurrentHostelId = room.HostelId;
                student.CurrentRoomNumber = room.RoomNumber;
                student.IsBoarding = true;
                student.LastCheckInTime = DateTime.Now;
                student.IsCurrentlyInHostel = true;
                student.AssignmentDate = DateTime.Now;
                student.ProbationEndDate = DateTime.Now.AddDays(7); // 7-day probation
                student.IsTemporaryAssignment = false;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Student has been successfully checked in.";
            return RedirectToAction(nameof(Details), new { id = id });
        }

        // POST: Booking/CheckOut/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> CheckOut(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Student)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Only allow check-out for checked-in bookings
            if (booking.Status != BookingStatus.CheckedIn)
            {
                TempData["ErrorMessage"] = "Only checked-in bookings can be checked out.";
                return RedirectToAction(nameof(Details), new { id = id });
            }

            booking.Status = BookingStatus.CheckedOut;

            // Update the room and student information
            var room = booking.Room;
            var student = booking.Student;

            if (room != null && student != null)
            {
                // Update room status and occupancy
                room.CurrentOccupancy -= 1;
                
                if (room.CurrentOccupancy <= 0)
                {
                    room.Status = RoomStatus.Available;
                    room.CurrentOccupancy = 0; // Ensure it never goes below 0
                }
                else
                {
                    room.Status = RoomStatus.PartiallyAssigned;
                }

                // Update student record - properly handle nullable properties
                // Check your ApplicationUser model to see if these are nullable or not
                // If they're not nullable, use an appropriate default value
                if (student.CurrentHostelId.HasValue) // This assumes CurrentHostelId is int?
                {
                    student.CurrentHostelId = null;
                }
                
                student.CurrentRoomNumber = string.Empty; // Using empty string instead of null if non-nullable
                student.IsBoarding = false;
                student.LastCheckOutTime = DateTime.Now;
                student.IsCurrentlyInHostel = false;
                student.AssignmentDate = null; // Assuming this is DateTime?
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Student has been successfully checked out.";
            return RedirectToAction(nameof(Details), new { id = id });
        }

        // POST: Booking/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            // Get current user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }

            // Check permissions - only allow admins, wardens, or the student who made the booking
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            var isWarden = await _userManager.IsInRoleAsync(currentUser, "Warden");

            if (!isAdmin && !isWarden && booking.UserId != currentUser.Id)
            {
                return Forbid();
            }

            // Only allow cancellation for pending or approved bookings
            if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Approved)
            {
                TempData["ErrorMessage"] = "Only pending or approved bookings can be cancelled.";
                return RedirectToAction(nameof(Details), new { id = id });
            }

            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();

            TempData["InfoMessage"] = "Booking has been cancelled.";
            
            // Redirect based on user role
            if (isAdmin || isWarden)
            {
                return RedirectToAction(nameof(ManageBookings));
            }
            else
            {
                return RedirectToAction(nameof(MyBookings));
            }
        }
    }
}