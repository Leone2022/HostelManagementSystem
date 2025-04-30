using HostelMS.Models;
using HostelMS.Services;
using HostelMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HostelMS.Controllers
{
    [Authorize(Roles = "Admin,Dean,Warden")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailService _emailService;

        public StudentController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _emailService = emailService;
        }

        // GET: Student
        public async Task<IActionResult> Index()
        {
            var students = await _userManager.Users
                .Include(u => u.CurrentHostel)
                .Where(u => u.UserRole == "Student" && u.IsApproved)
                .ToListAsync();

            ViewBag.Hostels = await _context.Hostels
                .Where(h => h.IsActive)
                .OrderBy(h => h.Name)
                .ToListAsync();

            return View(students);
        }

        // GET: Student/PendingApprovals
        public async Task<IActionResult> PendingApprovals()
        {
            var pendingStudents = await _userManager.Users
                .Where(u => u.UserRole == "Student" && !u.IsApproved)
                .OrderByDescending(u => u.RegistrationDate)
                .ToListAsync();

            return View(pendingStudents);
        }
        
        // GET: Student/Review/{id}
        public async Task<IActionResult> Review(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var student = await _userManager.FindByIdAsync(id);
            if (student == null || student.UserRole != "Student" || student.IsApproved)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Student/SendApprovalNotification/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendApprovalNotification(string id)
        {
            var student = await _userManager.FindByIdAsync(id);
            if (student == null || !student.IsApproved)
            {
                return NotFound();
            }

            // Send email notification
            string subject = "Bugema University Hostel - Account Approved";
            string message = $@"
                <html>
                <body>
                    <h2>Bugema University Hostel Management System</h2>
                    <p>Dear {student.FirstName} {student.LastName},</p>
                    <p>Your account has been approved. You can now log in to the Hostel Management System.</p>
                    <p>Student ID: {student.StudentId}</p>
                    <p>Email: {student.Email}</p>
                    <p>Please use your registered email and password to log in.</p>
                    <p><a href='https://hostelms.bugema.ac.ug/Account/Login?userType=student'>Click here to login</a></p>
                    <p>Best regards,<br>Hostel Management Team<br>Bugema University</p>
                </body>
                </html>";

            if (!string.IsNullOrEmpty(student.Email))
            {
                await _emailService.SendEmailAsync(student.Email, subject, message);
            }

            TempData["SuccessMessage"] = $"Approval notification sent to {student.Email}";
            return RedirectToAction("Details", new { id = student.Id });
        }

        // GET: Student/Search
        public async Task<IActionResult> Search(string searchTerm, int? hostelId, bool? verificationStatus, bool? approvalStatus)
        {
            var query = _userManager.Users
                .Include(u => u.CurrentHostel)
                .Where(u => u.UserRole == "Student");

            // Add approval status filter
            if (approvalStatus.HasValue)
            {
                query = query.Where(u => u.IsApproved == approvalStatus.Value);
            }
            else
            {
                // By default, show only approved students
                query = query.Where(u => u.IsApproved);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(u => 
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm) ||
                    (u.StudentId != null && u.StudentId.ToLower().Contains(searchTerm)) ||
                    u.Email.ToLower().Contains(searchTerm)
                );
            }

            if (hostelId.HasValue)
            {
                query = query.Where(u => u.CurrentHostelId == hostelId.Value);
            }

            if (verificationStatus.HasValue)
            {
                query = query.Where(u => u.IsVerified == verificationStatus.Value);
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.HostelId = hostelId;
            ViewBag.VerificationStatus = verificationStatus;
            ViewBag.ApprovalStatus = approvalStatus;
            ViewBag.Hostels = await _context.Hostels
                .Where(h => h.IsActive)
                .OrderBy(h => h.Name)
                .ToListAsync();

            var students = await query.ToListAsync();
            return View("Index", students);
        }

        // GET: Student/Details/{id}
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var student = await _userManager.Users
                .Include(u => u.CurrentHostel)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            var studentActivities = await _context.StudentActivities
                .Where(a => a.UserId == id)
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .ToListAsync();

            ViewBag.RecentActivities = studentActivities;

            return View(student);
        }

        // GET: Student/Assign/{id}
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> Assign(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var student = await _userManager.FindByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            // Only approved students can be assigned to rooms
            if (!student.IsApproved)
            {
                TempData["ErrorMessage"] = "The student must be approved before assigning a room.";
                return RedirectToAction("Details", new { id = student.Id });
            }

            var viewModel = new StudentAssignViewModel
            {
                Student = student,
                Hostels = await _context.Hostels
                    .Where(h => h.IsActive)
                    .OrderBy(h => h.Name)
                    .ToListAsync()
            };

            // If student already has a hostel assigned, preselect it
            if (student.CurrentHostelId.HasValue)
            {
                viewModel.SelectedHostelId = student.CurrentHostelId;
                viewModel.SelectedRoomNumber = student.CurrentRoomNumber;
                
                // Load available rooms for the selected hostel
                await LoadRoomsForHostel(viewModel);
            }

            return View(viewModel);
        }

                 // POST: Student/Delete
            [HttpPost]
            [ValidateAntiForgeryToken]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Delete(string id)
            {
                if (string.IsNullOrEmpty(id))
                {
                    return NotFound();
                }

                var student = await _userManager.FindByIdAsync(id);
                if (student == null)
                {
                    return NotFound();
                }

                if (student.UserRole != "Student")
                {
                    TempData["ErrorMessage"] = "Only student accounts can be deleted through this interface.";
                    return RedirectToAction(nameof(Index));
                }

                try
                {
                    // Check if student is assigned to a room and update room occupancy
                    if (student.CurrentHostelId.HasValue && !string.IsNullOrEmpty(student.CurrentRoomNumber))
                    {
                        var room = await _context.Rooms
                            .FirstOrDefaultAsync(r => r.HostelId == student.CurrentHostelId && 
                                                r.RoomNumber == student.CurrentRoomNumber);
                        
                        if (room != null)
                        {
                            // Decrease room occupancy
                            room.CurrentOccupancy = Math.Max(0, room.CurrentOccupancy - 1);
                            
                            // Update room status based on new occupancy
                            if (room.CurrentOccupancy == 0)
                            {
                                room.Status = RoomStatus.Available;
                            }
                            else if (room.CurrentOccupancy < room.Capacity)
                            {
                                room.Status = RoomStatus.PartiallyAssigned;
                            }
                            
                            _context.Update(room);
                            await _context.SaveChangesAsync();
                        }
                    }

                    // Delete the student user account
                    var result = await _userManager.DeleteAsync(student);
                    
                    if (result.Succeeded)
                    {
                        // Log the activity
                        TempData["SuccessMessage"] = $"Student {student.FirstName} {student.LastName} has been successfully deleted.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Error deleting student: {string.Join(", ", result.Errors.Select(e => e.Description))}";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error deleting student: {ex.Message}";
                }

                return RedirectToAction(nameof(Index));
            }

        // Helper method to load rooms for a hostel
        private async Task LoadRoomsForHostel(StudentAssignViewModel model)
        {
            if (model.SelectedHostelId.HasValue)
            {
                var rooms = await _context.Rooms
                    .Where(r => r.HostelId == model.SelectedHostelId &&
                               (r.Status == RoomStatus.Available || r.Status == RoomStatus.PartiallyAssigned) &&
                               r.CurrentOccupancy < r.Capacity)
                    .OrderBy(r => r.RoomNumber)
                    .ToListAsync();
                
                // Store the rooms in ViewBag for dropdown population
                ViewBag.AvailableRooms = rooms;
            }
        }

        // AJAX endpoint to get available rooms
        [HttpGet]
        public async Task<IActionResult> GetAvailableRooms(int hostelId)
        {
            var rooms = await _context.Rooms
                .Where(r => r.HostelId == hostelId &&
                           (r.Status == RoomStatus.Available || r.Status == RoomStatus.PartiallyAssigned) &&
                           r.CurrentOccupancy < r.Capacity)
                .Select(r => new {
                    roomId = r.RoomId,
                    roomNumber = r.RoomNumber,
                    type = r.Type.ToString(),
                    capacity = r.Capacity,
                    currentOccupancy = r.CurrentOccupancy,
                    price = r.PricePerSemester
                })
                .OrderBy(r => r.roomNumber)
                .ToListAsync();
                
            return Json(rooms);
        }
    }
}