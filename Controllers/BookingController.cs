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

        // GET: Booking - Show available hostels
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Index()
        {
            var privateHostels = await _context.Hostels
                .Where(h => h.IsActive && h.ManagementType == ManagementType.PrivatelyManaged)
                .OrderBy(h => h.Name)
                .ToListAsync();

            return View(privateHostels);
        }

        // GET: Booking/Create/5 - Show booking form for specific hostel
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

            if (!hostel.Rooms.Any())
            {
                TempData["ErrorMessage"] = "Sorry, there are no available rooms in this hostel.";
                return RedirectToAction(nameof(Index));
            }

            var student = await _userManager.GetUserAsync(User);
            if (student == null)
            {
                return NotFound();
            }

            // Check for existing active bookings
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
                CheckOutDate = DateTime.Today.AddMonths(4),
                UserId = student.Id
            };

            return View(model);
        }

        // POST: Booking/Create - Process booking with course validation
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            // Course validation
            if (string.IsNullOrEmpty(model.CourseSelection))
            {
                ModelState.AddModelError("CourseSelection", "Please select your course.");
            }
            else if (IsCourseRestricted(model.CourseSelection))
            {
                TempData["ErrorMessage"] = $"Unfortunately, students enrolled in {GetCourseDisplayName(model.CourseSelection)} cannot book accommodation through this system. You are required to stay in the school premises. Please visit the Office of the Dean of Students to be assigned a room.";
                
                await LoadHostelData(model);
                return View(model);
            }

            // Date validation
            if (model.CheckInDate < DateTime.Today)
            {
                ModelState.AddModelError("CheckInDate", "Check-in date cannot be in the past.");
            }

            if (model.CheckOutDate <= model.CheckInDate)
            {
                ModelState.AddModelError("CheckOutDate", "Check-out date must be after check-in date.");
            }

            if (model.CheckOutDate > model.CheckInDate.AddYears(1))
            {
                ModelState.AddModelError("CheckOutDate", "Booking period cannot exceed one year.");
            }

            if (ModelState.IsValid)
            {
                var room = await _context.Rooms
                    .Include(r => r.Hostel)
                    .FirstOrDefaultAsync(r => r.RoomId == model.RoomId);

                if (room == null)
                {
                    TempData["ErrorMessage"] = "The selected room does not exist.";
                    return RedirectToAction(nameof(Index));
                }

                if (room.Status == RoomStatus.FullyAssigned || room.Status == RoomStatus.UnderMaintenance)
                {
                    TempData["ErrorMessage"] = "This room is no longer available for booking.";
                    return RedirectToAction(nameof(Create), new { id = model.HostelId });
                }

                // Payment validation
                if (model.Amount < (room.PricePerSemester * 0.5m))
                {
                    ModelState.AddModelError("Amount", $"Payment amount should be at least 50% of the room price (UGX {room.PricePerSemester * 0.5m:N0}).");
                    await LoadHostelData(model);
                    return View(model);
                }

                // Create booking
                var booking = new Booking
                {
                    UserId = model.UserId,
                    RoomId = model.RoomId,
                    BookingDate = DateTime.Now,
                    CheckInDate = model.CheckInDate,
                    CheckOutDate = model.CheckOutDate,
                    TotalAmount = room.PricePerSemester,
                    Status = BookingStatus.Pending,
                    Comments = string.IsNullOrEmpty(model.Comments) 
                        ? $"Course: {GetCourseDisplayName(model.CourseSelection)}" 
                        : $"Course: {GetCourseDisplayName(model.CourseSelection)}\n{model.Comments}",
                    Course = model.CourseSelection
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
                    Notes = $"Payment for {room.Hostel?.Name ?? "Unknown Hostel"}, Room {room.RoomNumber}\nCourse: {GetCourseDisplayName(model.CourseSelection)}"
                };

                // Handle file upload
                if (model.PaymentProof != null)
                {
                    try
                    {
                        string uniqueFileName = $"{Guid.NewGuid()}_{model.PaymentProof.FileName}";
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "payments");
                        
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
                    catch (Exception)
                    {
                        TempData["WarningMessage"] = "Booking submitted successfully, but there was an issue uploading the payment proof. Please contact support.";
                    }
                }

                _context.Add(payment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Your booking request for {GetCourseDisplayName(model.CourseSelection)} has been submitted successfully! You will be notified once it's approved.";
                return RedirectToAction(nameof(MyBookings));
            }

            await LoadHostelData(model);
            return View(model);
        }

        // GET: Booking/MyBookings - Show student's bookings
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
                .ThenInclude(r => r.Hostel)
                .Where(b => b.UserId == student.Id)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Booking/ManageBookings - Admin booking management with course filtering
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> ManageBookings(
            string status = "Pending", 
            string searchString = null, 
            int? hostelId = null, 
            DateTime? dateFilter = null, 
            string courseFilter = null)
        {
            var query = _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hostel)
                .AsQueryable();

            // Status filter
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<BookingStatus>(status, out var bookingStatus))
            {
                query = query.Where(b => b.Status == bookingStatus);
            }

            // Search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(b => 
                    (b.Student != null && (
                        b.Student.FirstName.Contains(searchString) || 
                        b.Student.LastName.Contains(searchString) || 
                        b.Student.StudentId.Contains(searchString)
                    )) || 
                    (b.Room != null && b.Room.RoomNumber.Contains(searchString)) ||
                    (!string.IsNullOrEmpty(b.Course) && b.Course.Contains(searchString)));
            }

            // Hostel filter
            if (hostelId.HasValue)
            {
                query = query.Where(b => b.Room != null && b.Room.HostelId == hostelId);
            }

            // Course filter
            if (!string.IsNullOrEmpty(courseFilter))
            {
                query = query.Where(b => !string.IsNullOrEmpty(b.Course) && b.Course.Contains(courseFilter));
            }

            // Date filter
            if (dateFilter.HasValue)
            {
                DateTime filterDate = dateFilter.Value.Date;
                query = query.Where(b => b.BookingDate.Date == filterDate || 
                                        b.CheckInDate.Date == filterDate || 
                                        b.CheckOutDate.Date == filterDate);
            }

            // Get data for dropdowns
            var hostels = await _context.Hostels.OrderBy(h => h.Name).ToListAsync();
            var courses = await _context.Bookings
                .Where(b => !string.IsNullOrEmpty(b.Course))
                .Select(b => b.Course)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var pendingCount = await _context.Bookings
                .CountAsync(b => b.Status == BookingStatus.Pending);

            ViewBag.Hostels = hostels;
            ViewBag.Courses = courses;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentFilter = searchString;
            ViewBag.SelectedHostel = hostelId;
            ViewBag.CourseFilter = courseFilter;
            ViewBag.DateFilter = dateFilter?.ToString("yyyy-MM-dd");
            ViewBag.PendingCount = pendingCount;

            var bookings = await query
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Booking/Details/5 - View booking details
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hostel)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

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

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == booking.BookingId);

            ViewBag.Payment = payment;
            ViewBag.CourseDisplayName = !string.IsNullOrEmpty(booking.Course) 
                ? GetCourseDisplayName(booking.Course) 
                : "Not specified";

            return View(booking);
        }

        // GET: Booking/ProcessBooking/5 - Admin approval interface
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> ProcessBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hostel)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null || booking.Status != BookingStatus.Pending)
            {
                TempData["ErrorMessage"] = "Only pending bookings can be processed.";
                return RedirectToAction(nameof(ManageBookings));
            }

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == booking.BookingId);

            var model = new BookingProcessViewModel
            {
                BookingId = booking.BookingId,
                Booking = booking,
                Payment = payment,
                IsApproved = false
            };

            ViewBag.CourseDisplayName = !string.IsNullOrEmpty(booking.Course) 
                ? GetCourseDisplayName(booking.Course)
                : "Not specified";

            return View(model);
        }

        // POST: Booking/ProcessBooking - Process approval/rejection
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> ProcessBooking(int BookingId, bool IsApproved, string RejectionReason)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == BookingId);

            if (booking == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }

            if (IsApproved)
            {
                var room = booking.Room;
                if (room == null)
                {
                    TempData["ErrorMessage"] = "Room information not available.";
                    return RedirectToAction(nameof(ProcessBooking), new { id = BookingId });
                }

                // Room capacity validation
                if (room.Type == RoomType.Single)
                {
                    var existingApprovals = await _context.Bookings
                        .CountAsync(b => b.RoomId == room.RoomId && 
                                        (b.Status == BookingStatus.Approved || b.Status == BookingStatus.CheckedIn));

                    if (existingApprovals > 0)
                    {
                        TempData["ErrorMessage"] = "Cannot approve booking. This is a single room that already has an approved booking.";
                        return RedirectToAction(nameof(ProcessBooking), new { id = BookingId });
                    }
                }

                if (room.CurrentOccupancy >= room.Capacity)
                {
                    TempData["ErrorMessage"] = "Cannot approve booking. The room is already fully occupied.";
                    return RedirectToAction(nameof(ProcessBooking), new { id = BookingId });
                }

                // Check for earlier pending bookings
                var earlierPendingBookings = await _context.Bookings
                    .AnyAsync(b => b.RoomId == room.RoomId && 
                                b.Status == BookingStatus.Pending && 
                                b.BookingDate < booking.BookingDate);

                if (earlierPendingBookings)
                {
                    TempData["WarningMessage"] = "Note: There are earlier pending booking requests for this room. Consider processing those first.";
                }

                // Approve booking
                booking.Status = BookingStatus.Approved;
                booking.ApprovedBy = currentUser.Id;
                booking.ApprovalDate = DateTime.Now;
                
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
                if (string.IsNullOrWhiteSpace(RejectionReason))
                {
                    TempData["ErrorMessage"] = "Rejection reason is required when rejecting a booking.";
                    return RedirectToAction(nameof(ProcessBooking), new { id = BookingId });
                }

                booking.Status = BookingStatus.Rejected;
                booking.RejectionReason = RejectionReason;
                
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.BookingId == booking.BookingId);
                
                if (payment != null)
                {
                    payment.Status = PaymentStatus.Failed;
                    payment.Notes = $"{payment.Notes ?? ""}\nRejected by {currentUser.FirstName} {currentUser.LastName}: {RejectionReason}";
                }

                TempData["InfoMessage"] = "Booking has been rejected.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageBookings));
        }

        // POST: Booking/CheckIn/5 - Check in student
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> CheckIn(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Student)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null || booking.Status != BookingStatus.Approved)
            {
                TempData["ErrorMessage"] = "Only approved bookings can be checked in.";
                return RedirectToAction(nameof(Details), new { id });
            }

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
                    return RedirectToAction(nameof(Details), new { id });
                }
            }

            if (room != null && room.CurrentOccupancy >= room.Capacity)
            {
                TempData["ErrorMessage"] = "Cannot check in student. The room is already at full capacity.";
                return RedirectToAction(nameof(Details), new { id });
            }

            booking.Status = BookingStatus.CheckedIn;

            var student = booking.Student;
            if (room != null && student != null)
            {
                room.CurrentOccupancy += 1;
                
                if (room.CurrentOccupancy >= room.Capacity)
                {
                    room.Status = RoomStatus.FullyAssigned;
                }
                else if (room.CurrentOccupancy > 0)
                {
                    room.Status = RoomStatus.PartiallyAssigned;
                }

                student.CurrentHostelId = room.HostelId;
                student.CurrentRoomNumber = room.RoomNumber;
                student.IsBoarding = true;
                student.LastCheckInTime = DateTime.Now;
                student.IsCurrentlyInHostel = true;
                student.AssignmentDate = DateTime.Now;
                student.ProbationEndDate = DateTime.Now.AddDays(7);
                student.IsTemporaryAssignment = false;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Student has been successfully checked in.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Booking/CheckOut/5 - Check out student
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> CheckOut(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Student)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null || booking.Status != BookingStatus.CheckedIn)
            {
                TempData["ErrorMessage"] = "Only checked-in bookings can be checked out.";
                return RedirectToAction(nameof(Details), new { id });
            }

            booking.Status = BookingStatus.CheckedOut;

            var room = booking.Room;
            var student = booking.Student;

            if (room != null && student != null)
            {
                room.CurrentOccupancy -= 1;
                
                if (room.CurrentOccupancy <= 0)
                {
                    room.Status = RoomStatus.Available;
                    room.CurrentOccupancy = 0;
                }
                else
                {
                    room.Status = RoomStatus.PartiallyAssigned;
                }

                student.CurrentHostelId = null;
                student.CurrentRoomNumber = string.Empty;
                student.IsBoarding = false;
                student.LastCheckOutTime = DateTime.Now;
                student.IsCurrentlyInHostel = false;
                student.AssignmentDate = null;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Student has been successfully checked out.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Booking/Cancel/5 - Cancel booking
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

            if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Approved)
            {
                TempData["ErrorMessage"] = "Only pending or approved bookings can be cancelled.";
                return RedirectToAction(nameof(Details), new { id });
            }

            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();

            TempData["InfoMessage"] = "Booking has been cancelled.";
            
            if (isAdmin || isWarden)
            {
                return RedirectToAction(nameof(ManageBookings));
            }
            else
            {
                return RedirectToAction(nameof(MyBookings));
            }
        }

        // GET: Booking/DownloadReceipt/5 - Generate receipt
        [HttpGet]
        public async Task<IActionResult> DownloadReceipt(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hostel)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Check permissions
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");
            var isWarden = User.IsInRole("Warden");

            if (!isAdmin && !isWarden && booking.UserId != currentUser.Id)
            {
                return Forbid();
            }

            // Get payment information
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == booking.BookingId);

            // Generate HTML receipt
            var html = GenerateReceiptHtml(booking, payment);

            // Return HTML view for printing
            return Content(html, "text/html");
        }

        // GET: Booking/CourseStatistics - Course analytics dashboard
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> CourseStatistics()
        {
            var courseStats = await _context.Bookings
                .Where(b => !string.IsNullOrEmpty(b.Course))
                .GroupBy(b => b.Course)
                .Select(g => new
                {
                    CourseCode = g.Key,
                    TotalBookings = g.Count(),
                    PendingBookings = g.Count(b => b.Status == BookingStatus.Pending),
                    ApprovedBookings = g.Count(b => b.Status == BookingStatus.Approved),
                    CheckedInBookings = g.Count(b => b.Status == BookingStatus.CheckedIn),
                    RejectedBookings = g.Count(b => b.Status == BookingStatus.Rejected),
                    CancelledBookings = g.Count(b => b.Status == BookingStatus.Cancelled)
                })
                .ToListAsync();

            var courseStatsWithNames = courseStats.Select(stat => new
            {
                stat.CourseCode,
                CourseName = GetCourseDisplayName(stat.CourseCode),
                stat.TotalBookings,
                stat.PendingBookings,
                stat.ApprovedBookings,
                stat.CheckedInBookings,
                stat.RejectedBookings,
                stat.CancelledBookings,
                IsRestricted = IsCourseRestricted(stat.CourseCode)
            }).OrderByDescending(x => x.TotalBookings).ToList();

            ViewBag.TotalBookings = courseStats.Sum(x => x.TotalBookings);
            ViewBag.TotalPending = courseStats.Sum(x => x.PendingBookings);
            ViewBag.TotalApproved = courseStats.Sum(x => x.ApprovedBookings);
            ViewBag.TotalCheckedIn = courseStats.Sum(x => x.CheckedInBookings);
            ViewBag.TotalRejected = courseStats.Sum(x => x.RejectedBookings);
            ViewBag.TotalCancelled = courseStats.Sum(x => x.CancelledBookings);

            return View(courseStatsWithNames);
        }

        // GET: Booking/RestrictedCourseInfo - Information page for restricted courses
        [Authorize(Roles = "Student")]
        public IActionResult RestrictedCourseInfo()
        {
            var restrictedCourses = new List<string>
            {
                "BSc_Nursing", "Dip_Nursing", "Cert_Nursing",
                "Cert_IT", "Cert_Childhood", "Cert_Networks", 
                "Cert_Office", "Cert_Repair", "HEC_Sciences"
            };

            var courseNames = restrictedCourses.Select(courseCode => GetCourseDisplayName(courseCode)).ToList();

            ViewBag.RestrictedCourses = courseNames;
            return View();
        }

        // GET: Booking/BulkApprove - Batch processing interface
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkApprove()
        {
            var pendingBookings = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hostel)
                .Where(b => b.Status == BookingStatus.Pending)
                .OrderBy(b => b.BookingDate)
                .ToListAsync();

            var bookingsWithCourseNames = pendingBookings.Select(booking => new
            {
                Booking = booking,
                CourseDisplayName = !string.IsNullOrEmpty(booking.Course) 
                    ? GetCourseDisplayName(booking.Course)
                    : "Not specified"
            }).ToList();

            ViewBag.BookingsWithCourseNames = bookingsWithCourseNames;
            return View(pendingBookings);
        }

        // POST: Booking/BulkApproveSelected - Process bulk approval
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkApproveSelected(List<int> selectedBookingIds)
        {
            if (selectedBookingIds == null || !selectedBookingIds.Any())
            {
                TempData["ErrorMessage"] = "No bookings selected for approval.";
                return RedirectToAction(nameof(BulkApprove));
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }

            int approvedCount = 0;
            int errorCount = 0;
            var errors = new List<string>();

            foreach (var bookingId in selectedBookingIds)
            {
                var booking = await _context.Bookings
                    .Include(b => b.Room)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.Status == BookingStatus.Pending);

                if (booking == null)
                {
                    errors.Add($"Booking ID {bookingId} not found or not pending.");
                    errorCount++;
                    continue;
                }

                var room = booking.Room;
                if (room == null)
                {
                    errors.Add($"Booking ID {bookingId}: Room information not available.");
                    errorCount++;
                    continue;
                }

                // Room capacity validation
                if (room.Type == RoomType.Single)
                {
                    var existingApprovals = await _context.Bookings
                        .CountAsync(b => b.RoomId == room.RoomId && 
                                        (b.Status == BookingStatus.Approved || b.Status == BookingStatus.CheckedIn));

                    if (existingApprovals > 0)
                    {
                        errors.Add($"Booking ID {bookingId}: Single room already has approved booking.");
                        errorCount++;
                        continue;
                    }
                }

                if (room.CurrentOccupancy >= room.Capacity)
                {
                    errors.Add($"Booking ID {bookingId}: Room is already at full capacity.");
                    errorCount++;
                    continue;
                }

                // Approve the booking
                booking.Status = BookingStatus.Approved;
                booking.ApprovedBy = currentUser.Id;
                booking.ApprovalDate = DateTime.Now;
                
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.BookingId == booking.BookingId);
                
                if (payment != null)
                {
                    payment.Status = PaymentStatus.Completed;
                    payment.Notes = $"{payment.Notes ?? ""}\nBulk approved by {currentUser.FirstName} {currentUser.LastName} on {DateTime.Now}";
                }

                approvedCount++;
            }

            await _context.SaveChangesAsync();

            if (approvedCount > 0)
            {
                TempData["SuccessMessage"] = $"Successfully approved {approvedCount} booking(s).";
            }

            if (errorCount > 0)
            {
                TempData["ErrorMessage"] = $"{errorCount} booking(s) could not be approved. " + string.Join(" ", errors.Take(3));
                if (errors.Count > 3)
                {
                    TempData["ErrorMessage"] += $" And {errors.Count - 3} more errors.";
                }
            }

            return RedirectToAction(nameof(ManageBookings));
        }

        // API endpoint to check if a course is restricted (for AJAX calls)
        [HttpGet]
        [Authorize]
        public IActionResult CheckCourseRestriction(string courseCode)
        {
            return Json(new 
            { 
                isRestricted = IsCourseRestricted(courseCode),
                courseName = GetCourseDisplayName(courseCode),
                message = IsCourseRestricted(courseCode) ? "Unfortunately, you cannot book accommodation through this system. Under this course, you are required to stay in the school premises. Please visit the Office of the Dean of Students to be assigned a room." : null
            });
        }

        // Private helper method to load hostel data for form reloading
        private async Task LoadHostelData(BookingCreateViewModel viewModel)
        {
            var hostel = await _context.Hostels
                .Include(h => h.Rooms.Where(r => r.Status == RoomStatus.Available || r.Status == RoomStatus.PartiallyAssigned))
                .FirstOrDefaultAsync(h => h.HostelId == viewModel.HostelId);

            if (hostel != null)
            {
                viewModel.HostelName = hostel.Name;
                viewModel.AvailableRooms = hostel.Rooms
                    .Select(room => new SelectListItem
                    {
                        Value = room.RoomId.ToString(),
                        Text = $"Room {room.RoomNumber} - {room.Type} - UGX {room.PricePerSemester:N0}/semester - {room.Capacity - room.CurrentOccupancy} bed(s) available"
                    })
                    .ToList();
            }
        }

        // Private helper method to generate receipt HTML
        private string GenerateReceiptHtml(Booking booking, Payment payment)
        {
            var courseDisplayName = !string.IsNullOrEmpty(booking.Course) 
                ? GetCourseDisplayName(booking.Course) 
                : "Not specified";

            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Booking Receipt #{booking.BookingId}</title>
    <style>
        body {{ 
            font-family: Arial, sans-serif; 
            margin: 20px; 
            color: #333;
            background: white;
        }}
        .receipt-container {{ 
            max-width: 800px; 
            margin: 0 auto; 
            border: 2px solid #2c3e50;
            background: white;
        }}
        .header {{ 
            background: linear-gradient(135deg, #2c3e50, #3498db); 
            color: white; 
            padding: 30px; 
            text-align: center; 
        }}
        .header h1 {{ 
            margin: 0; 
            font-size: 28px; 
            font-weight: bold;
        }}
        .header p {{ 
            margin: 5px 0; 
            opacity: 0.9; 
        }}
        .receipt-number {{ 
            background: rgba(255,255,255,0.2); 
            display: inline-block; 
            padding: 8px 16px; 
            border-radius: 20px; 
            margin-top: 10px;
            font-weight: bold;
            font-size: 16px;
        }}
        .content {{ 
            padding: 30px; 
        }}
        .section {{ 
            margin-bottom: 25px; 
            border-bottom: 1px solid #eee; 
            padding-bottom: 20px; 
        }}
        .section:last-child {{ 
            border-bottom: none; 
        }}
        .section-title {{ 
            color: #2c3e50; 
            font-size: 18px; 
            font-weight: bold; 
            margin-bottom: 15px; 
            display: flex; 
            align-items: center;
        }}
        .info-grid {{ 
            display: grid; 
            grid-template-columns: 1fr 1fr; 
            gap: 15px; 
        }}
        .info-item {{ 
            display: flex; 
            margin-bottom: 8px; 
        }}
        .info-label {{ 
            font-weight: bold; 
            width: 140px; 
            color: #555; 
        }}
        .info-value {{ 
            color: #333; 
        }}
        .status-badge {{ 
            display: inline-block; 
            padding: 6px 12px; 
            border-radius: 15px; 
            font-size: 12px; 
            font-weight: bold; 
            text-transform: uppercase; 
        }}
        .status-approved {{ background: #d4edda; color: #155724; }}
        .status-pending {{ background: #fff3cd; color: #856404; }}
        .status-rejected {{ background: #f8d7da; color: #721c24; }}
        .status-checkedin {{ background: #cce5ff; color: #0066cc; }}
        .status-cancelled {{ background: #e2e3e5; color: #383d41; }}
        .amount-highlight {{ 
            background: #f8f9fa; 
            border: 2px solid #2c3e50; 
            border-radius: 8px; 
            padding: 15px; 
            text-align: center; 
            margin: 20px 0; 
        }}
        .amount-highlight .amount {{ 
            font-size: 24px; 
            font-weight: bold; 
            color: #2c3e50; 
        }}
        .footer {{ 
            background: #f8f9fa; 
            padding: 20px; 
            text-align: center; 
            font-size: 12px; 
            color: #666; 
            border-top: 1px solid #eee; 
        }}
        .watermark {{ 
            position: absolute; 
            top: 50%; 
            left: 50%; 
            transform: translate(-50%, -50%) rotate(-45deg); 
            font-size: 100px; 
            color: rgba(0,0,0,0.05); 
            z-index: -1; 
            font-weight: bold; 
        }}
        .restriction-notice {{ 
            background: #fff3cd; 
            border: 1px solid #ffeaa7; 
            border-radius: 5px; 
            padding: 15px; 
            margin: 15px 0; 
        }}
        .restriction-notice .title {{ 
            font-weight: bold; 
            color: #856404; 
            margin-bottom: 5px; 
        }}
        @media print {{
            body {{ margin: 0; }}
            .receipt-container {{ border: none; box-shadow: none; }}
            .no-print {{ display: none; }}
        }}
    </style>
</head>
<body>
    <div class='receipt-container'>
        <div class='watermark'>RECEIPT</div>
        
        <div class='header'>
            <h1>🏨 UNIVERSITY HOSTEL SERVICES</h1>
            <p>Booking Confirmation Receipt</p>
            <div class='receipt-number'>Receipt #{booking.BookingId:D6}</div>
        </div>
        
        <div class='content'>
            <!-- Booking Information -->
            <div class='section'>
                <div class='section-title'>
                    📋 Booking Information
                </div>
                <div class='info-grid'>
                    <div>
                        <div class='info-item'>
                            <span class='info-label'>Booking ID:</span>
                            <span class='info-value'>#{booking.BookingId}</span>
                        </div>
                        <div class='info-item'>
                            <span class='info-label'>Booking Date:</span>
                            <span class='info-value'>{booking.BookingDate:dd MMM yyyy HH:mm}</span>
                        </div>
                        <div class='info-item'>
                            <span class='info-label'>Check-in Date:</span>
                            <span class='info-value'>{booking.CheckInDate:dd MMM yyyy}</span>
                        </div>
                        <div class='info-item'>
                            <span class='info-label'>Check-out Date:</span>
                            <span class='info-value'>{booking.CheckOutDate:dd MMM yyyy}</span>
                        </div>
                    </div>
                    <div>
                        <div class='info-item'>
                            <span class='info-label'>Status:</span>
                            <span class='info-value'>
                                <span class='status-badge status-{booking.Status.ToString().ToLower()}'>{booking.Status}</span>
                            </span>
                        </div>
                        <div class='info-item'>
                            <span class='info-label'>Course:</span>
                            <span class='info-value'>{courseDisplayName}</span>
                        </div>
                        {(booking.Status == BookingStatus.Approved && !string.IsNullOrEmpty(booking.ApprovedBy) ? $@"
                        <div class='info-item'>
                            <span class='info-label'>Approved By:</span>
                            <span class='info-value'>{booking.ApprovedBy}</span>
                        </div>
                        <div class='info-item'>
                            <span class='info-label'>Approval Date:</span>
                            <span class='info-value'>{booking.ApprovalDate?.ToString("dd MMM yyyy") ?? ""}</span>
                        </div>" : "")}
                    </div>
                </div>
            </div>
            
            <!-- Student Information -->
            <div class='section'>
                <div class='section-title'>
                    👨‍🎓 Student Information
                </div>
                <div class='info-grid'>
                    <div>
                        <div class='info-item'>
                            <span class='info-label'>Name:</span>
                            <span class='info-value'>{booking.Student?.FirstName} {booking.Student?.LastName}</span>
                        </div>
                        <div class='info-item'>
                            <span class='info-label'>Student ID:</span>
                            <span class='info-value'>{booking.Student?.StudentId ?? "Not available"}</span>
                        </div>
                        <div class='info-item'>
                            <span class='info-label'>Email:</span>
                            <span class='info-value'>{booking.Student?.Email ?? "Not available"}</span>
                        </div>
                    </div>
                    <div>
                        <div class='info-item'>
                            <span class='info-label'>Phone:</span>
                            <span class='info-value'>{booking.Student?.PhoneNumber ?? "Not available"}</span>
                        </div>
                        <div class='info-item'>
                            <span class='info-label'>Course (Profile):</span>
                            <span class='info-value'>{booking.Student?.Course ?? "Not available"}</span>
                        </div>
                        <div class='info-item'>
                            <span class='info-label'>Year:</span>
                            <span class='info-value'>{booking.Student?.Year ?? "Not available"}</span>
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Hostel & Room Information -->
            <div class='section'>
                <div class='section-title'>
                    🏠 Accommodation Details
                </div>
                <div class='info-grid'>
                    <div>
                        <div class='info-item'>
                            <span class='info-label'>Hostel:</span>
                            <span class='info-value'>{booking.Room?.Hostel?.Name ?? "Not available"}</span>
                        </div>
                        <div class='info-item'>
                            <span class='info-label'>Location:</span>
                            <span class='info-value'>{booking.Room?.Hostel?.Location ?? "Not available"}</span>
                        </div>
                        <div class='info-item'>
                            <span class='info-label'>Gender:</span>
                            <span class='info-value'>{booking.Room?.Hostel?.Gender.ToString() ?? "Not available"}</span>
                        </div>
                    </div>
                    <div>
                        <div class='info-item'>
                            <span class='info-label'>Room Number:</span>
                            <span class='info-value'>{booking.Room?.RoomNumber ?? "Not available"}</span>
                        </div>
                        <div class='info-item'>
                            <span class='info-label'>Room Type:</span>
                            <span class='info-value'>{booking.Room?.Type.ToString() ?? "Not available"}</span>
                        </div>
                        <div class='info-item'>
                            <span class='info-label'>Capacity:</span>
                            <span class='info-value'>{booking.Room?.Capacity.ToString() ?? "Not available"} students</span>
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Course Restriction Notice -->
            {(booking.IsCourseRestricted() ? $@"
            <div class='restriction-notice'>
                <div class='title'>⚠️ Course Restriction Notice</div>
                This booking is for a restricted course ({courseDisplayName}). Students in restricted courses may need to visit the Office of the Dean of Students for special accommodation arrangements.
            </div>" : "")}
            
            <!-- Payment Information -->
            <div class='section'>
                <div class='section-title'>
                    💰 Payment Details
                </div>
                {(payment != null ? $@"
                <div class='info-grid'>
                    <div>
                        <div class='info-item'>
                            <span class='info-label'>Payment Date:</span>
                            <span class='info-value'>{payment.PaymentDate:dd MMM yyyy HH:mm}</span>
                        </div>
                        <div class='info-item'>
                            <span class='info-label'>Method:</span>
                            <span class='info-value'>{payment.Method}</span>
                        </div>
                        <div class='info-item'>
                            <span class='info-label'>Transaction Ref:</span>
                            <span class='info-value'>{payment.TransactionReference}</span>
                        </div>
                    </div>
                    <div>
                        <div class='info-item'>
                            <span class='info-label'>Status:</span>
                            <span class='info-value'>
                                <span class='status-badge status-{payment.Status.ToString().ToLower()}'>{payment.Status}</span>
                            </span>
                        </div>
                        <div class='info-item'>
                            <span class='info-label'>Receipt Number:</span>
                            <span class='info-value'>{payment.ReceiptNumber ?? "Pending"}</span>
                        </div>
                    </div>
                </div>" : "<p>Payment information not available.</p>")}
                
                <div class='amount-highlight'>
                    <div>Total Amount Paid</div>
                    <div class='amount'>UGX {booking.TotalAmount:N0}</div>
                </div>
            </div>
            
            {(!string.IsNullOrEmpty(booking.Comments) ? $@"
            <!-- Additional Comments -->
            <div class='section'>
                <div class='section-title'>
                    💬 Comments
                </div>
                <p>{booking.Comments}</p>
            </div>" : "")}
        </div>
        
        <div class='footer'>
            <p><strong>University Hostel Management System</strong></p>
            <p>Generated on {DateTime.Now:dd MMM yyyy HH:mm} | This is an electronically generated receipt.</p>
            <p>For queries, contact: hostel.admin@university.ac.ug | Phone: +256-XXX-XXXXXX</p>
            <div class='no-print' style='margin-top: 15px;'>
                <button onclick='window.print()' style='background: #2c3e50; color: white; border: none; padding: 10px 20px; border-radius: 5px; cursor: pointer;'>
                    🖨️ Print Receipt
                </button>
                <button onclick='window.close()' style='background: #6c757d; color: white; border: none; padding: 10px 20px; border-radius: 5px; cursor: pointer; margin-left: 10px;'>
                    ❌ Close
                </button>
            </div>
        </div>
    </div>
</body>
</html>";

            return html;
        }

        // Private helper method to validate course restrictions
        private static bool IsCourseRestricted(string courseCode)
        {
            if (string.IsNullOrEmpty(courseCode))
                return false;

            var restrictedCourses = new HashSet<string>
            {
                // ALL NURSING COURSES
                "BSc_Nursing", "Dip_Nursing", "Cert_Nursing",
                
                // ALL CERTIFICATE PROGRAMS
                "Cert_IT", "Cert_Childhood", "Cert_Networks", 
                "Cert_Office", "Cert_Repair",
                
                // BRIDGING COURSES
                "HEC_Sciences"
            };

            return restrictedCourses.Contains(courseCode);
        }

        // Private helper method to get course display names
        private static string GetCourseDisplayName(string courseCode)
        {
            if (string.IsNullOrEmpty(courseCode))
                return "Not specified";

            var courseNames = new Dictionary<string, string>
            {
                // UNDERGRADUATE DEGREE PROGRAMS
                // School of Business
                { "BBA_Accounting", "Bachelor of Business Administration in Accounting" },
                { "BBA_Finance", "Bachelor of Business Administration in Finance" },
                { "BBA_Insurance", "Bachelor of Business Administration in Insurance" },
                { "BBA_Marketing", "Bachelor of Business Administration in Marketing" },
                { "BBA_HRM", "Bachelor of Business Administration in Human Resource Management" },
                { "BBA_Economics", "Bachelor of Business Administration in Economics" },
                { "BBA_Entrepreneurship", "Bachelor of Business Administration in Entrepreneurship" },
                { "BBA_Management", "Bachelor of Business Administration in Management" },
                { "BBA_Secretarial", "Bachelor of Business Administration in Secretarial Studies & Office Management" },
                { "BBA_BIS", "Bachelor of Business Administration in Business Information Systems" },
                { "BBA_Trade", "Bachelor of Business Administration in International Trade" },
                
                // School of Computing and Informatics
                { "BSc_Networks", "Bachelor of Science in Computer Networks and System Administration" },
                { "BSc_Software", "Bachelor of Science in Software Engineering and Application Development" },
                { "BBA_BIS_Computing", "Bachelor of Business Administration in Business Information Systems" },
                
                // School of Social Sciences
                { "BA_Development", "Bachelor of Arts in Development Studies" },
                { "BPA_Management", "Bachelor of Public Administration and Management" },
                { "BA_SocialWork", "Bachelor of Arts in Social Work and Social Administration" },
                { "BSc_Counseling", "Bachelor of Science in Counseling and Psychology" },
                { "BSA_Sociology", "Bachelor of Social Administration and Sociology" },
                
                // School of Education
                { "BA_Education", "Bachelor of Arts with Education" },
                { "BSc_Education", "Bachelor of Science with Education" },
                
                // School of Theology
                { "B_Theology", "Bachelor of Theology" },
                { "BA_Religious", "Bachelor of Arts in Religious Studies with Chaplaincy" },
                { "B_Development_Ministry", "Bachelor of Development Ministry" },
                { "B_Evangelism", "Bachelor of Evangelism and Church Growth" },
                { "B_Biblical_Counseling", "Bachelor of Biblical Counseling" },
                
                // School of Health Sciences
                { "BSc_Nursing", "Bachelor of Nursing Science" }, // RESTRICTED
                { "BSc_Food", "Bachelor of Science in Food Technology and Human Nutrition" },
                
                // School of Agricultural and Environmental Sciences
                { "BSc_Agriculture", "Bachelor of Science in Agriculture" },
                { "BSc_Agribusiness", "Bachelor of Science in Agribusiness Innovation and Management" },
                { "BSc_Environmental", "Bachelor of Science in Environmental Science" },
                { "BSc_Statistics", "Bachelor of Science in Statistics" },
                { "BSc_Biochemistry", "Bachelor of Science in Biochemistry" },
                
                // POSTGRADUATE DEGREE PROGRAMS
                // Master's Programs
                { "MBA", "Master of Business Administration" },
                { "MA_Education", "Master of Arts in Education Management" },
                { "MA_Development", "Master of Arts in Development Studies" },
                { "MA_English", "Master of Arts in English Literature" },
                { "MSc_Counseling", "Master of Science in Counseling Psychology" },
                { "MPH", "Master of Public Health" },
                { "MSc_IS", "Master of Science in Information Systems" },
                { "MSc_Software_Masters", "Master of Science in Software Engineering and Application Development" },
                { "MSc_Security", "Master of Science in Network Security" },
                { "MSc_Rural", "Master of Science in Rural Development" },
                { "M_LocalGov", "Master in Local Government Management" },
                { "MIT_Management", "Master of Information Technology and Management" },
                { "MSc_Palliative", "Master of Science in Palliative Care" },
                { "MIS", "Master of Information Science" },
                
                // Doctoral Programs
                { "PhD_DevEducation", "Doctor of Philosophy in Developmental Education" },
                { "PhD_Rural", "Doctor of Philosophy in Rural Development" },
                { "PhD_Communication", "Doctor of Philosophy in Developmental Communication" },
                { "PhD_EducationMgmt", "PhD in Educational Management" },
                { "PhD_Environmental", "PhD in Environmental Management" },
                
                // DIPLOMA PROGRAMS
                { "Dip_BA_Accounting", "Diploma in Business Administration in Accounting" },
                { "Dip_Secretarial", "Diploma in Secretarial Studies & Office Management" },
                { "Dip_Development", "Diploma in Development Studies" },
                { "Dip_Education", "Diploma in Education" },
                { "Dip_Nursing", "Diploma in Nursing" }, // RESTRICTED
                { "Dip_Food", "Diploma in Food Science & Processing Technology" },
                { "Dip_Biomedical", "Diploma in Biomedical Engineering & Technology" },
                { "Dip_Lab", "Diploma in Science Laboratory Technology" },
                { "Dip_Forensics", "Diploma in Computer Forensics" },
                { "Dip_SocialWork", "Diploma in Social Work and Social Administration" },
                { "Dip_Counseling", "Diploma in Counseling" },
                
                // CERTIFICATE PROGRAMS (ALL RESTRICTED)
                { "Cert_IT", "Certificate in Information Technology" }, // RESTRICTED
                { "Cert_Nursing", "Certificate in Nursing Program" }, // RESTRICTED
                { "Cert_Childhood", "Certificate in Early Childhood Education" }, // RESTRICTED
                { "Cert_Networks", "Certificate in Small Business Computer Networks" }, // RESTRICTED
                { "Cert_Office", "Certificate in Office Automation and Data Management" }, // RESTRICTED
                { "Cert_Repair", "Certificate in Computer Repair and Maintenance" }, // RESTRICTED
                
                // BRIDGING COURSES (ALL RESTRICTED)
                { "HEC_Sciences", "Higher Education Certificate (Biological and Physical Sciences)" } // RESTRICTED
            };

            return courseNames.TryGetValue(courseCode, out var displayName) ? displayName : courseCode;
        }
    }
}