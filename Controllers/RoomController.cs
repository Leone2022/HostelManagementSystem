using HostelMS.Models;
using HostelMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HostelMS.Controllers
{
    [Authorize(Roles = "Admin,Warden")]
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoomController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Room/Index
        public async Task<IActionResult> Index(string searchTerm = null, int? hostelId = null, RoomStatus? status = null)
        {
            var query = _context.Rooms.Include(r => r.Hostel).AsQueryable();
            
            // Apply filters
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(r => 
                    r.RoomNumber.ToLower().Contains(searchTerm) || 
                    r.Hostel.Name.ToLower().Contains(searchTerm));
            }
            
            if (hostelId.HasValue)
            {
                query = query.Where(r => r.HostelId == hostelId.Value);
            }
            
            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }
            
            var rooms = await query.OrderBy(r => r.Hostel.Name).ThenBy(r => r.RoomNumber).ToListAsync();
            
            // Get all hostels for filter dropdown
            ViewBag.Hostels = await _context.Hostels.Where(h => h.IsActive).OrderBy(h => h.Name).ToListAsync();
            ViewBag.AllHostels = await _context.Hostels.OrderBy(h => h.Name).ToListAsync();
            ViewBag.SearchTerm = searchTerm;
            ViewBag.HostelId = hostelId;
            ViewBag.Status = status;
            
            return View(rooms);
        }

        // GET: Room/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Hostel)
                .Include(r => r.OccupyingStudents)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // GET: Room/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Hostel)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Room/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room model)
        {
            if (id != model.RoomId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var room = await _context.Rooms.FindAsync(id);
                    if (room == null)
                    {
                        return NotFound();
                    }

                    // Ensure the capacity isn't less than current occupancy
                    if (model.Capacity < room.CurrentOccupancy)
                    {
                        ModelState.AddModelError("Capacity", $"Capacity cannot be less than current occupancy ({room.CurrentOccupancy})");
                        return View(model);
                    }

                    room.RoomNumber = model.RoomNumber;
                    room.Type = model.Type;
                    room.Capacity = model.Capacity;
                    room.Description = model.Description;
                    room.PricePerSemester = model.PricePerSemester;
                    room.NeedsAttention = model.NeedsAttention;
                    
                    // Allow status changes in the UI now
                    room.Status = model.Status;

                    _context.Update(room);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Room {room.RoomNumber} was updated successfully.";
                    return RedirectToAction("Details", new { id = room.RoomId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(model.RoomId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            
            // If we got this far, something failed - reload the hostel
            if (model.HostelId > 0)
            {
                model.Hostel = await _context.Hostels.FindAsync(model.HostelId);
            }
            
            return View(model);
        }

        // GET: Room/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Hostel)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            // Check if this room has any occupants
            bool hasOccupants = room.CurrentOccupancy > 0;
            if (hasOccupants)
            {
                TempData["ErrorMessage"] = "Cannot delete this room as it currently has occupants.";
                return RedirectToAction("Details", new { id = room.RoomId });
            }

            return View(room);
        }

        // POST: Room/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            // Check if this room has any occupants
            if (room.CurrentOccupancy > 0)
            {
                TempData["ErrorMessage"] = "Cannot delete this room as it currently has occupants.";
                return RedirectToAction("Details", new { id = room.RoomId });
            }

            int hostelId = room.HostelId;
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Room {room.RoomNumber} was successfully deleted.";
            return RedirectToAction("RoomLayout", "Hostel", new { id = hostelId });
        }

        // GET: Room/Assign/5
        public async Task<IActionResult> Assign(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Hostel)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            // Get current occupants of the room
            var currentOccupants = await _context.Users
                .Where(u => u.CurrentHostelId == room.HostelId && 
                       u.CurrentRoomNumber == room.RoomNumber)
                .ToListAsync();

            // Get available students (not assigned to any room)
            var availableStudents = await _context.Users
                .Where(u => u.UserRole == "Student" && 
                       u.IsApproved &&
                       (u.CurrentHostelId == null || u.CurrentRoomNumber == null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();

            var model = new RoomAssignmentViewModel
            {
                Hostel = room.Hostel,
                Room = room,
                CurrentOccupants = currentOccupants,
                AvailableStudents = availableStudents
            };

            return View(model);
        }

        // POST: Room/AssignStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignStudent(int roomId, string studentId, bool isTemporary, string notes)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
            {
                return NotFound();
            }

            var student = await _userManager.FindByIdAsync(studentId);
            if (student == null)
            {
                return NotFound();
            }

            // Check if room has available space
            if (room.CurrentOccupancy >= room.Capacity)
            {
                TempData["ErrorMessage"] = "This room is already at full capacity.";
                return RedirectToAction(nameof(Assign), new { id = roomId });
            }

            // If student was previously assigned to a different room, update that room's occupancy
            if (student.CurrentHostelId.HasValue && !string.IsNullOrEmpty(student.CurrentRoomNumber) &&
                (student.CurrentHostelId != room.HostelId || student.CurrentRoomNumber != room.RoomNumber))
            {
                var previousRoom = await _context.Rooms
                    .FirstOrDefaultAsync(r => r.HostelId == student.CurrentHostelId && 
                                        r.RoomNumber == student.CurrentRoomNumber);
                
                if (previousRoom != null)
                {
                    previousRoom.CurrentOccupancy = Math.Max(0, previousRoom.CurrentOccupancy - 1);
                    
                    // Update room status based on new occupancy
                    previousRoom.Status = previousRoom.CurrentOccupancy == 0 
                        ? RoomStatus.Available 
                        : (previousRoom.CurrentOccupancy < previousRoom.Capacity 
                            ? RoomStatus.PartiallyAssigned 
                            : RoomStatus.FullyAssigned);
                    
                    _context.Update(previousRoom);
                }
            }

            // Update student's hostel assignment
            student.CurrentHostelId = room.HostelId;
            student.CurrentRoomNumber = room.RoomNumber;
            student.AssignmentDate = DateTime.Now;
            student.IsTemporaryAssignment = isTemporary;
            
            student.ProbationEndDate = isTemporary 
                ? DateTime.Now.AddDays(30)  // Default 30-day probation period
                : null;

            // Update room occupancy
            room.CurrentOccupancy += 1;
            
            // Update room status based on new occupancy
            room.Status = room.CurrentOccupancy >= room.Capacity 
                ? RoomStatus.FullyAssigned 
                : RoomStatus.PartiallyAssigned;
            
            _context.Update(room);

            // Save changes
            await _context.SaveChangesAsync();
            var result = await _userManager.UpdateAsync(student);
            
            if (result.Succeeded)
            {
                // Log activity
                var activity = new StudentActivity
                {
                    UserId = student.Id,
                    UserName = $"{student.FirstName} {student.LastName}",
                    ActivityType = "Room Assignment",
                    Description = $"Assigned to {room.RoomNumber} in {room.Hostel?.Name ?? "Unknown Hostel"}",
                    Timestamp = DateTime.Now
                };
                _context.StudentActivities.Add(activity);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Student {student.FirstName} {student.LastName} has been successfully assigned to room {room.RoomNumber}.";
                return RedirectToAction(nameof(Assign), new { id = roomId });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    TempData["ErrorMessage"] = error.Description;
                }
                return RedirectToAction(nameof(Assign), new { id = roomId });
            }
        }

        // POST: Room/UnassignStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnassignStudent(int roomId, string studentId, string reason, string notes)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
            {
                return NotFound();
            }

            var student = await _userManager.FindByIdAsync(studentId);
            if (student == null)
            {
                return NotFound();
            }

            // Only unassign if the student is assigned to this room
            if (student.CurrentHostelId == room.HostelId && student.CurrentRoomNumber == room.RoomNumber)
            {
                // Update student's hostel assignment
                student.CurrentHostelId = null;
                student.CurrentRoomNumber = null;
                student.AssignmentDate = null;
                student.IsTemporaryAssignment = false;
                student.ProbationEndDate = null;

                // Update room occupancy
                room.CurrentOccupancy = Math.Max(0, room.CurrentOccupancy - 1);
                
                // Update room status based on new occupancy
                if (room.CurrentOccupancy == 0)
                {
                    room.Status = RoomStatus.Available;
                }
                else
                {
                    room.Status = RoomStatus.PartiallyAssigned;
                }
                
                _context.Update(room);

                // Save changes
                await _context.SaveChangesAsync();
                var result = await _userManager.UpdateAsync(student);
                
                if (result.Succeeded)
                {
                    // Log activity
                    var activity = new StudentActivity
                    {
                        UserId = student.Id,
                        UserName = $"{student.FirstName} {student.LastName}",
                        ActivityType = "Room Unassignment",
                        Description = $"Unassigned from {room.RoomNumber} in {room.Hostel?.Name ?? "Unknown Hostel"} - Reason: {reason}",
                        Timestamp = DateTime.Now
                    };
                    _context.StudentActivities.Add(activity);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Student {student.FirstName} {student.LastName} has been successfully unassigned from room {room.RoomNumber}.";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        TempData["ErrorMessage"] = error.Description;
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "This student is not currently assigned to this room.";
            }

            return RedirectToAction(nameof(Assign), new { id = roomId });
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.RoomId == id);
        }
    }
}