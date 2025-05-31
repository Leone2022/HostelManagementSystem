using HostelMS.Models;
using HostelMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace HostelMS.Controllers
{
    [Authorize(Roles = "Admin,Dean,Warden")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public StudentController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: Student
        public async Task<IActionResult> Index(
            string searchTerm = "",
            int? hostelId = null,
            string courseFilter = null,
            int? yearFilter = null,
            string statusFilter = null,
            int page = 1,
            int pageSize = 50)
        {
            var query = BuildStudentQuery(searchTerm, hostelId, courseFilter, yearFilter, statusFilter);

            var totalCount = await query.CountAsync();
            var students = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.HostelId = hostelId;
            ViewBag.CourseFilter = courseFilter;
            ViewBag.YearFilter = yearFilter;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            ViewBag.Hostels = await _context.Hostels
                .Where(h => h.IsActive)
                .OrderBy(h => h.Name)
                .ToListAsync();

            ViewBag.Courses = await _context.Users
                .Where(u => !string.IsNullOrEmpty(u.StudentId) && !string.IsNullOrEmpty(u.Course))
                .Select(u => u.Course)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return View(students);
        }

        private IQueryable<ApplicationUser> BuildStudentQuery(
            string searchTerm,
            int? hostelId,
            string courseFilter,
            int? yearFilter,
            string statusFilter)
        {
            // Filter students by checking if they have StudentId (indicates they are students)
            var query = _context.Users
                .Include(u => u.CurrentHostel)
                .Where(u => !string.IsNullOrEmpty(u.StudentId)) // Students have StudentId
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u =>
                    (u.FirstName != null && u.FirstName.Contains(searchTerm)) ||
                    (u.LastName != null && u.LastName.Contains(searchTerm)) ||
                    (u.StudentId != null && u.StudentId.Contains(searchTerm)) ||
                    (u.Email != null && u.Email.Contains(searchTerm)));
            }

            if (hostelId.HasValue)
            {
                query = query.Where(u => u.CurrentHostelId == hostelId);
            }

            if (!string.IsNullOrEmpty(courseFilter))
            {
                query = query.Where(u => u.Course == courseFilter);
            }

            if (yearFilter.HasValue)
            {
                query = query.Where(u => u.Year == yearFilter.ToString());
            }

            switch (statusFilter)
            {
                case "verified":
                    query = query.Where(u => u.EmailConfirmed);
                    break;
                case "unverified":
                    query = query.Where(u => !u.EmailConfirmed);
                    break;
                case "assigned":
                    query = query.Where(u => u.CurrentHostelId != null);
                    break;
                case "unassigned":
                    query = query.Where(u => u.CurrentHostelId == null);
                    break;
            }

            return query.OrderBy(u => u.FirstName ?? "").ThenBy(u => u.LastName ?? "");
        }

        // GET: Student/Search - Handle search functionality
        public async Task<IActionResult> Search(
            string searchTerm = "",
            int? hostelId = null,
            bool? verificationStatus = null,
            bool? approvalStatus = null)
        {
            // Build query for students (users with StudentId)
            var query = _context.Users
                .Include(u => u.CurrentHostel)
                .Where(u => !string.IsNullOrEmpty(u.StudentId))
                .AsQueryable();

            // Apply search term filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u =>
                    (u.FirstName != null && u.FirstName.Contains(searchTerm)) ||
                    (u.LastName != null && u.LastName.Contains(searchTerm)) ||
                    (u.StudentId != null && u.StudentId.Contains(searchTerm)) ||
                    (u.Email != null && u.Email.Contains(searchTerm)));
            }

            // Apply hostel filter
            if (hostelId.HasValue)
            {
                query = query.Where(u => u.CurrentHostelId == hostelId);
            }

            // Apply verification status filter
            if (verificationStatus.HasValue)
            {
                query = query.Where(u => u.EmailConfirmed == verificationStatus.Value);
            }

            var students = await query
                .OrderBy(u => u.FirstName ?? "")
                .ThenBy(u => u.LastName ?? "")
                .ToListAsync();

            // Pass filter values to view
            ViewBag.SearchTerm = searchTerm;
            ViewBag.HostelId = hostelId;
            ViewBag.VerificationStatus = verificationStatus;
            ViewBag.ApprovalStatus = approvalStatus;

            ViewBag.Hostels = await _context.Hostels
                .Where(h => h.IsActive)
                .OrderBy(h => h.Name)
                .ToListAsync();

            return View("Index", students);
        }

        // GET: Student/Details/{id}
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var student = await _userManager.Users
                .Include(u => u.CurrentHostel)
                .FirstOrDefaultAsync(u => u.Id == id && !string.IsNullOrEmpty(u.StudentId));

            if (student == null)
                return NotFound();

            // Get student's bookings
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.Hostel)
                .Where(b => b.UserId == id)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentBookings = bookings;

            return View(student);
        }

        // GET: Student/PendingApprovals - Show students pending approval
        [HttpGet]
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> PendingApprovals()
        {
            var pendingStudents = await _context.Users
                .Where(u => !string.IsNullOrEmpty(u.StudentId) && !u.EmailConfirmed)
                .OrderBy(u => u.RegistrationDate)
                .ToListAsync();

            return View(pendingStudents);
        }

        // POST: Student/Approve/{id} - Approve a student
        [HttpPost]
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> Approve(string id)
        {
            var student = await _context.Users.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            student.EmailConfirmed = true;
            student.IsApproved = true;
            student.ApprovalDate = DateTime.Now;
            student.ApprovedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Student {student.FirstName} {student.LastName} has been approved successfully.";
            return RedirectToAction(nameof(PendingApprovals));
        }

        // POST: Student/Reject/{id} - Reject a student
        [HttpPost]
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> Reject(string id, string reason)
        {
            var student = await _context.Users.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            // For rejection, you might want to add a rejection reason field to your model
            // For now, we'll just not approve them
            student.EmailConfirmed = false;
            student.IsApproved = false;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Student {student.FirstName} {student.LastName} application has been rejected.";
            return RedirectToAction(nameof(PendingApprovals));
        }

        // POST: Student/BulkApprove - Approve multiple students
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkApprove(List<string> selectedStudents)
        {
            if (selectedStudents == null || !selectedStudents.Any())
            {
                TempData["ErrorMessage"] = "No students selected for approval.";
                return RedirectToAction(nameof(PendingApprovals));
            }

            var students = await _context.Users
                .Where(u => selectedStudents.Contains(u.Id))
                .ToListAsync();

            foreach (var student in students)
            {
                student.EmailConfirmed = true;
                student.IsApproved = true;
                student.ApprovalDate = DateTime.Now;
                student.ApprovedBy = User.Identity?.Name;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{students.Count} students have been approved successfully.";
            return RedirectToAction(nameof(PendingApprovals));
        }

        // GET: Student/DownloadStudentReport - Excel Report
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> DownloadStudentReport(
            string searchTerm = "", 
            int? hostelId = null, 
            string courseFilter = null,
            int? yearFilter = null,
            string statusFilter = null)
        {
            var students = await BuildStudentQuery(searchTerm, hostelId, courseFilter, yearFilter, statusFilter)
                .ToListAsync();

            var csvContent = GenerateStudentCSV(students);
            var fileName = $"Student_Report_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            
            return File(System.Text.Encoding.UTF8.GetBytes(csvContent), "text/csv", fileName);
        }

        // GET: Student/GenerateStudentPDF - PDF Report  
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> GenerateStudentPDF(
            string searchTerm = "", 
            int? hostelId = null, 
            string courseFilter = null,
            int? yearFilter = null,
            string statusFilter = null)
        {
            var students = await BuildStudentQuery(searchTerm, hostelId, courseFilter, yearFilter, statusFilter)
                .ToListAsync();
            
            var html = GenerateStudentReportHTML(students, searchTerm, hostelId);
            
            // Return HTML for now (you can integrate a PDF library later)
            return Content(html, "text/html");
        }

        // Generate CSV content - ONLY USING EXISTING PROPERTIES
        private string GenerateStudentCSV(List<ApplicationUser> students)
        {
            var csv = new StringBuilder();
            
            // Header - Only properties that exist in ApplicationUser
            csv.AppendLine("Student ID,First Name,Last Name,Email,Phone,Course,Year,Current Hostel,Room Number,Verification Status,Registration Date");
            
            // Data rows
            foreach (var student in students)
            {
                csv.AppendLine($"\"{student.StudentId ?? "N/A"}\"," +
                              $"\"{student.FirstName ?? "N/A"}\"," +
                              $"\"{student.LastName ?? "N/A"}\"," +
                              $"\"{student.Email ?? "N/A"}\"," +
                              $"\"{student.PhoneNumber ?? "N/A"}\"," +
                              $"\"{student.Course ?? "N/A"}\"," +
                              $"\"{student.Year ?? "N/A"}\"," +
                              $"\"{student.CurrentHostel?.Name ?? "Not Assigned"}\"," +
                              $"\"{student.CurrentRoomNumber ?? "N/A"}\"," +
                              $"\"{(student.EmailConfirmed ? "Verified" : "Not Verified")}\"," +
                              $"\"{student.RegistrationDate:yyyy-MM-dd}\"");
            }
            
            return csv.ToString();
        }

        // Generate detailed HTML report - ONLY USING EXISTING PROPERTIES
        private string GenerateStudentReportHTML(List<ApplicationUser> students, string searchTerm, int? hostelId)
        {
            var hostelName = hostelId.HasValue ? 
                _context.Hostels.FirstOrDefault(h => h.HostelId == hostelId.Value)?.Name ?? "Unknown Hostel" : 
                "All Hostels";

            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Student Directory Report</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; color: #333; background: white; }}
        .header {{ text-align: center; border-bottom: 2px solid #2c3e50; padding-bottom: 20px; margin-bottom: 30px; }}
        .header h1 {{ color: #2c3e50; margin: 0; font-size: 24px; }}
        .header h2 {{ color: #7f8c8d; margin: 5px 0; font-size: 18px; }}
        .info-box {{ background: #ecf0f1; padding: 15px; border-radius: 5px; margin-bottom: 20px; font-size: 14px; }}
        .stats {{ display: flex; justify-content: space-around; margin-bottom: 30px; }}
        .stat {{ text-align: center; }}
        .stat h3 {{ color: #2c3e50; margin: 0; font-size: 18px; }}
        .stat p {{ color: #7f8c8d; margin: 5px 0 0 0; font-size: 12px; }}
        table {{ width: 100%; border-collapse: collapse; margin-bottom: 30px; font-size: 12px; }}
        th, td {{ border: 1px solid #bdc3c7; padding: 6px; text-align: left; }}
        th {{ background: #34495e; color: white; }}
        tr:nth-child(even) {{ background: #f8f9fa; }}
        .status-verified {{ color: #27ae60; font-weight: bold; }}
        .status-pending {{ color: #e74c3c; font-weight: bold; }}
        .footer {{ text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #bdc3c7; font-size: 10px; color: #7f8c8d; }}
        @@media print {{
            body {{ margin: 0; }}
            .no-print {{ display: none; }}
        }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>🏫 UNIVERSITY HOSTEL MANAGEMENT SYSTEM</h1>
        <h2>Student Directory Report</h2>
        <p>Generated on {DateTime.Now:dd MMMM yyyy 'at' HH:mm}</p>
    </div>

    <div class='info-box'>
        <strong>Report Parameters:</strong><br>
        📊 Total Students: {students.Count}<br>
        🏠 Hostel Filter: {hostelName}<br>
        🔍 Search Term: {(!string.IsNullOrEmpty(searchTerm) ? searchTerm : "None")}<br>
        📅 Report Date: {DateTime.Now:yyyy-MM-dd HH:mm}
    </div>

    <div class='stats'>
        <div class='stat'>
            <h3>{students.Count}</h3>
            <p>Total Students</p>
        </div>
        <div class='stat'>
            <h3>{students.Count(s => s.EmailConfirmed)}</h3>
            <p>Verified</p>
        </div>
        <div class='stat'>
            <h3>{students.Count(s => s.CurrentHostelId.HasValue)}</h3>
            <p>Assigned to Hostels</p>
        </div>
        <div class='stat'>
            <h3>{students.Where(s => !string.IsNullOrEmpty(s.Course)).GroupBy(s => s.Course).Count()}</h3>
            <p>Different Courses</p>
        </div>
    </div>

    <table>
        <thead>
            <tr>
                <th>Student ID</th>
                <th>Full Name</th>
                <th>Course</th>
                <th>Year</th>
                <th>Contact</th>
                <th>Hostel Assignment</th>
                <th>Status</th>
            </tr>
        </thead>
        <tbody>";

            foreach (var student in students)
            {
                var verificationStatus = student.EmailConfirmed ? "Verified" : "Not Verified";
                var statusClass = student.EmailConfirmed ? "status-verified" : "status-pending";

                html += $@"
            <tr>
                <td><strong>{student.StudentId ?? "N/A"}</strong></td>
                <td>{student.FirstName ?? ""} {student.LastName ?? ""}</td>
                <td>{student.Course ?? "Not specified"}</td>
                <td>{student.Year ?? "N/A"}</td>
                <td>
                    📧 {student.Email ?? "N/A"}<br>
                    📱 {student.PhoneNumber ?? "N/A"}
                </td>
                <td>
                    {(student.CurrentHostel?.Name ?? "Not Assigned")}
                    {(!string.IsNullOrEmpty(student.CurrentRoomNumber) ? $"<br>Room: {student.CurrentRoomNumber}" : "")}
                </td>
                <td class='{statusClass}'>
                    {verificationStatus}
                </td>
            </tr>";
            }

            html += $@"
        </tbody>
    </table>

    <div class='footer'>
        <p><strong>University Hostel Management System</strong></p>
        <p>This report contains confidential student information. Handle with care.</p>
        <p>Generated by: {User.Identity?.Name ?? "System"} | System Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
    </div>
</body>
</html>";

            return html;
        }

        // POST: Student/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var student = await _userManager.FindByIdAsync(id);
            if (student == null || string.IsNullOrEmpty(student.StudentId))
                return NotFound();

            try
            {
                // Update room occupancy if assigned
                if (student.CurrentHostelId.HasValue && !string.IsNullOrEmpty(student.CurrentRoomNumber))
                {
                    var room = await _context.Rooms
                        .FirstOrDefaultAsync(r => r.HostelId == student.CurrentHostelId && 
                                                r.RoomNumber == student.CurrentRoomNumber);
                    
                    if (room != null)
                    {
                        room.CurrentOccupancy = Math.Max(0, room.CurrentOccupancy - 1);
                        _context.Update(room);
                        await _context.SaveChangesAsync();
                    }
                }

                var result = await _userManager.DeleteAsync(student);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Student deleted successfully";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = $"Error deleting student: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting student: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // AJAX: Get available rooms for hostel
        [HttpGet]
        public async Task<IActionResult> GetAvailableRooms(int hostelId)
        {
            var rooms = await _context.Rooms
                .Where(r => r.HostelId == hostelId &&
                           r.CurrentOccupancy < r.Capacity &&
                           r.Status == RoomStatus.Available)
                .Select(r => new 
                {
                    RoomId = r.RoomId,
                    RoomNumber = r.RoomNumber,
                    Type = r.Type.ToString(),
                    Capacity = r.Capacity,
                    Occupancy = r.CurrentOccupancy,
                    Available = r.Capacity - r.CurrentOccupancy,
                    Price = r.PricePerSemester
                })
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();
                
            return Json(rooms);
        }
    }
}