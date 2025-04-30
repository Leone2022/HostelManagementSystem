using HostelMS.Models;
using HostelMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HostelMS.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Redirect to role-specific dashboard
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToAction("AdminDashboard");
            }
            else if (await _userManager.IsInRoleAsync(user, "Warden"))
            {
                return RedirectToAction("WardenDashboard");
            }
            else if (await _userManager.IsInRoleAsync(user, "Dean"))
            {
                return RedirectToAction("DeanDashboard");
            }
            else if (await _userManager.IsInRoleAsync(user, "Student"))
            {
                return RedirectToAction("StudentDashboard");
            }
            else if (await _userManager.IsInRoleAsync(user, "Landlord"))
            {
                // Future implementation
                return RedirectToAction("LandlordDashboard");
            }

            // Default fallback
            return View();
        }

        // GET: Dashboard/StudentDashboard
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentDashboard()
        {
            var student = await _userManager.GetUserAsync(User);
            if (student == null)
            {
                return NotFound();
            }

            // Get current hostel and room if assigned
            Hostel? currentHostel = null;
            Room? currentRoom = null;

            if (student.CurrentHostelId.HasValue)
            {
                currentHostel = await _context.Hostels
                    .FirstOrDefaultAsync(h => h.HostelId == student.CurrentHostelId);

                if (!string.IsNullOrEmpty(student.CurrentRoomNumber))
                {
                    currentRoom = await _context.Rooms
                        .FirstOrDefaultAsync(r => r.HostelId == student.CurrentHostelId && 
                                               r.RoomNumber == student.CurrentRoomNumber);
                }
            }

            // Get active booking information
            var activeBooking = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hostel)
                .Where(b => b.UserId == student.Id && 
                          (b.Status == BookingStatus.Pending || 
                           b.Status == BookingStatus.Approved ||
                           b.Status == BookingStatus.CheckedIn))
                .OrderByDescending(b => b.BookingDate)
                .FirstOrDefaultAsync();

            bool hasPendingBooking = activeBooking != null && activeBooking.Status == BookingStatus.Pending;
            bool hasApprovedBooking = activeBooking != null && 
                (activeBooking.Status == BookingStatus.Approved || activeBooking.Status == BookingStatus.CheckedIn);

            // Get recent notifications
            var recentNotifications = await _context.Notifications
                .Where(n => n.RecipientId == student.Id)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Get available private hostels for booking
            var availableHostels = await _context.Hostels
                .Where(h => h.IsActive && h.ManagementType == ManagementType.PrivatelyManaged)
                .OrderBy(h => h.Name)
                .Take(6)
                .ToListAsync();

            // Get recent bookings
            var recentBookings = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hostel)
                .Where(b => b.UserId == student.Id)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToListAsync();

            // Create the view model
            var model = new StudentDashboardViewModel
            {
                Student = student,
                CurrentHostel = currentHostel,
                CurrentRoom = currentRoom,
                ActiveBooking = activeBooking,
                HasPendingBooking = hasPendingBooking,
                HasApprovedBooking = hasApprovedBooking,
                RecentNotifications = recentNotifications,
                AvailableHostels = availableHostels
            };

            ViewBag.RecentBookings = recentBookings;

            return View(model);
        }

        // GET: Dashboard/AdminDashboard
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            // Count statistics
            int totalStudents = await _context.Users
                .CountAsync(u => u.UserRole == "Student");

            int totalHostels = await _context.Hostels.CountAsync();
            int totalRooms = await _context.Rooms.CountAsync();
            int occupiedRooms = await _context.Rooms
                .CountAsync(r => r.Status == RoomStatus.FullyAssigned || r.Status == RoomStatus.PartiallyAssigned);

            int pendingBookings = await _context.Bookings
                .CountAsync(b => b.Status == BookingStatus.Pending);

            int maintenanceRequests = await _context.MaintenanceRequests
                .CountAsync(m => m.Status == MaintenanceStatus.Pending || m.Status == MaintenanceStatus.InProgress);

            // Recent bookings
            var recentBookings = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hostel)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToListAsync();

            // Recently registered students
            var recentStudents = await _context.Users
                .Where(u => u.UserRole == "Student" && !u.IsApproved)
                .OrderByDescending(u => u.RegistrationDate)
                .Take(5)
                .ToListAsync();

            // Rooms needing maintenance
            var maintenanceRooms = await _context.Rooms
                .Include(r => r.Hostel)
                .Where(r => r.NeedsAttention || r.Status == RoomStatus.UnderMaintenance)
                .OrderBy(r => r.Hostel.Name)
                .ThenBy(r => r.RoomNumber)
                .Take(5)
                .ToListAsync();

            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalHostels = totalHostels;
            ViewBag.TotalRooms = totalRooms;
            ViewBag.OccupiedRooms = occupiedRooms;
            ViewBag.PendingBookings = pendingBookings;
            ViewBag.MaintenanceRequests = maintenanceRequests;
            ViewBag.RecentBookings = recentBookings;
            ViewBag.RecentStudents = recentStudents;
            ViewBag.MaintenanceRooms = maintenanceRooms;

            return View();
        }

        // GET: Dashboard/WardenDashboard
        [Authorize(Roles = "Warden")]
        public async Task<IActionResult> WardenDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Get warden's hostel
            var hostel = await _context.Hostels
                .FirstOrDefaultAsync(h => h.WardenId == user.Id);

            if (hostel == null)
            {
                ViewBag.NoHostel = true;
                return View();
            }

            // Room statistics
            var rooms = await _context.Rooms
                .Where(r => r.HostelId == hostel.HostelId)
                .ToListAsync();

            int totalRooms = rooms.Count();
            int occupiedRooms = rooms.Count(r => r.Status == RoomStatus.FullyAssigned || r.Status == RoomStatus.PartiallyAssigned);
            int availableRooms = rooms.Count(r => r.Status == RoomStatus.Available);
            int maintenanceRooms = rooms.Count(r => r.Status == RoomStatus.UnderMaintenance || r.NeedsAttention);

            int totalCapacity = rooms.Sum(r => r.Capacity);
            int totalOccupancy = rooms.Sum(r => r.CurrentOccupancy);
            double occupancyRate = totalCapacity > 0 ? (double)totalOccupancy / totalCapacity * 100 : 0;

            // Current students
            var students = await _context.Users
                .Where(u => u.UserRole == "Student" && u.CurrentHostelId == hostel.HostelId)
                .OrderBy(u => u.CurrentRoomNumber)
                .ToListAsync();

            // Pending bookings for this hostel
            var pendingBookings = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Room)
                .Where(b => b.Room.HostelId == hostel.HostelId && b.Status == BookingStatus.Pending)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToListAsync();

            // Recent check-ins/check-outs
            var recentActivity = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Room)
                .Where(b => b.Room.HostelId == hostel.HostelId && 
                          (b.Status == BookingStatus.CheckedIn || b.Status == BookingStatus.CheckedOut))
                .OrderByDescending(b => b.Status == BookingStatus.CheckedIn ? b.ApprovalDate : b.CheckOutDate)
                .Take(5)
                .ToListAsync();

            // Maintenance requests
            var maintenanceRequests = await _context.MaintenanceRequests
                .Include(m => m.Room)
                .Include(m => m.ReportedBy)
                .Where(m => m.Room.HostelId == hostel.HostelId && 
                          (m.Status == MaintenanceStatus.Pending || m.Status == MaintenanceStatus.InProgress))
                .OrderByDescending(m => m.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.Hostel = hostel;
            ViewBag.TotalRooms = totalRooms;
            ViewBag.OccupiedRooms = occupiedRooms;
            ViewBag.AvailableRooms = availableRooms;
            ViewBag.MaintenanceRooms = maintenanceRooms;
            ViewBag.TotalCapacity = totalCapacity;
            ViewBag.TotalOccupancy = totalOccupancy;
            ViewBag.OccupancyRate = occupancyRate;
            ViewBag.Students = students;
            ViewBag.PendingBookings = pendingBookings;
            ViewBag.RecentActivity = recentActivity;
            ViewBag.MaintenanceRequests = maintenanceRequests;

            return View();
        }

        // GET: Dashboard/DeanDashboard
        [Authorize(Roles = "Dean")]
        public async Task<IActionResult> DeanDashboard()
        {
            // Hostels overview
            var hostels = await _context.Hostels
                .OrderBy(h => h.Name)
                .ToListAsync();

            // Calculate statistics
            int totalHostels = hostels.Count();
            int activeHostels = hostels.Count(h => h.IsActive);
            int privateHostels = hostels.Count(h => h.ManagementType == ManagementType.PrivatelyManaged);
            int institutionalHostels = hostels.Count(h => h.ManagementType == ManagementType.InstitutionallyManaged);

            // Room statistics
            int totalRooms = await _context.Rooms.CountAsync();
            int availableRooms = await _context.Rooms.CountAsync(r => r.Status == RoomStatus.Available);
            int occupiedRooms = await _context.Rooms.CountAsync(r => 
                r.Status == RoomStatus.PartiallyAssigned || r.Status == RoomStatus.FullyAssigned);
            int maintenanceRooms = await _context.Rooms.CountAsync(r => r.Status == RoomStatus.UnderMaintenance);

            int totalCapacity = await _context.Rooms.SumAsync(r => r.Capacity);
            int totalOccupancy = await _context.Rooms.SumAsync(r => r.CurrentOccupancy);
            double occupancyRate = totalCapacity > 0 ? (double)totalOccupancy / totalCapacity * 100 : 0;

            // Student statistics
            int totalStudents = await _context.Users.CountAsync(u => u.UserRole == "Student");
            int boardingStudents = await _context.Users.CountAsync(u => 
                u.UserRole == "Student" && u.IsBoarding && u.CurrentHostelId != null);
            int unassignedStudents = await _context.Users.CountAsync(u => 
                u.UserRole == "Student" && u.IsApproved && u.CurrentHostelId == null);
            int pendingApprovals = await _context.Users.CountAsync(u => 
                u.UserRole == "Student" && !u.IsApproved);

            // Recent bookings
            var recentBookings = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hostel)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToListAsync();

            // Maintenance requests
            var maintenanceRequests = await _context.MaintenanceRequests
                .Include(m => m.Room)
                .ThenInclude(r => r.Hostel)
                .Include(m => m.ReportedBy)
                .Where(m => m.Status == MaintenanceStatus.Pending || m.Status == MaintenanceStatus.InProgress)
                .OrderByDescending(m => m.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.Hostels = hostels;
            ViewBag.TotalHostels = totalHostels;
            ViewBag.ActiveHostels = activeHostels;
            ViewBag.PrivateHostels = privateHostels;
            ViewBag.InstitutionalHostels = institutionalHostels;
            ViewBag.TotalRooms = totalRooms;
            ViewBag.AvailableRooms = availableRooms;
            ViewBag.OccupiedRooms = occupiedRooms;
            ViewBag.MaintenanceRooms = maintenanceRooms;
            ViewBag.TotalCapacity = totalCapacity;
            ViewBag.TotalOccupancy = totalOccupancy;
            ViewBag.OccupancyRate = occupancyRate;
            ViewBag.TotalStudents = totalStudents;
            ViewBag.BoardingStudents = boardingStudents;
            ViewBag.UnassignedStudents = unassignedStudents;
            ViewBag.PendingApprovals = pendingApprovals;
            ViewBag.RecentBookings = recentBookings;
            ViewBag.MaintenanceRequests = maintenanceRequests;

            return View();
        }

        // GET: Dashboard/LandlordDashboard
        [Authorize(Roles = "Landlord")]
        public async Task<IActionResult> LandlordDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Get landlord's hostels
            var hostels = await _context.Hostels
                .Where(h => h.LandlordId == user.Id)
                .OrderBy(h => h.Name)
                .ToListAsync();

            if (!hostels.Any())
            {
                ViewBag.NoHostels = true;
                return View();
            }

            var hostelIds = hostels.Select(h => h.HostelId).ToList();

            // Room statistics
            var rooms = await _context.Rooms
                .Where(r => hostelIds.Contains(r.HostelId))
                .ToListAsync();

            int totalRooms = rooms.Count();
            int occupiedRooms = rooms.Count(r => r.Status == RoomStatus.FullyAssigned || r.Status == RoomStatus.PartiallyAssigned);
            int availableRooms = rooms.Count(r => r.Status == RoomStatus.Available);
            int maintenanceRooms = rooms.Count(r => r.Status == RoomStatus.UnderMaintenance || r.NeedsAttention);

            int totalCapacity = rooms.Sum(r => r.Capacity);
            int totalOccupancy = rooms.Sum(r => r.CurrentOccupancy);
            double occupancyRate = totalCapacity > 0 ? (double)totalOccupancy / totalCapacity * 100 : 0;

            // Current students
            var students = await _context.Users
                .Where(u => u.UserRole == "Student" && hostelIds.Contains(u.CurrentHostelId ?? 0))
                .OrderBy(u => u.CurrentHostelId)
                .ThenBy(u => u.CurrentRoomNumber)
                .ToListAsync();

            // Pending bookings for these hostels
            var pendingBookings = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hostel)
                .Where(b => hostelIds.Contains(b.Room.HostelId) && b.Status == BookingStatus.Pending)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToListAsync();

            // Recent bookings
            var recentBookings = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hostel)
                .Where(b => hostelIds.Contains(b.Room.HostelId))
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToListAsync();

            // Maintenance requests
            var maintenanceRequests = await _context.MaintenanceRequests
                .Include(m => m.Room)
                .ThenInclude(r => r.Hostel)
                .Include(m => m.ReportedBy)
                .Where(m => hostelIds.Contains(m.Room.HostelId) && 
                         (m.Status == MaintenanceStatus.Pending || m.Status == MaintenanceStatus.InProgress))
                .OrderByDescending(m => m.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.Hostels = hostels;
            ViewBag.TotalRooms = totalRooms;
            ViewBag.OccupiedRooms = occupiedRooms;
            ViewBag.AvailableRooms = availableRooms;
            ViewBag.MaintenanceRooms = maintenanceRooms;
            ViewBag.TotalCapacity = totalCapacity;
            ViewBag.TotalOccupancy = totalOccupancy;
            ViewBag.OccupancyRate = occupancyRate;
            ViewBag.Students = students;
            ViewBag.PendingBookings = pendingBookings;
            ViewBag.RecentBookings = recentBookings;
            ViewBag.MaintenanceRequests = maintenanceRequests;

            return View();
        }
    }
}