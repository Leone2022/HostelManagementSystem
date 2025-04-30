using HostelMS.Models;
using HostelMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HostelMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HostelController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<HostelController> _logger;

        public HostelController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment, ILogger<HostelController> logger)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        // GET: Hostel - Admin can view all hostels
        public async Task<IActionResult> Index(ManagementType? managementType = null, string searchTerm = null)
        {
            IQueryable<Hostel> query = _context.Hostels.Include(h => h.Rooms);
            
            if (managementType.HasValue)
            {
                query = query.Where(h => h.ManagementType == managementType.Value);
            }
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(h => 
                    h.Name.ToLower().Contains(searchTerm) || 
                    h.Location.ToLower().Contains(searchTerm) || 
                    (h.HostelCode != null && h.HostelCode.ToLower().Contains(searchTerm)));
            }
            
            var hostels = await query.ToListAsync();
            
            ViewBag.ManagementType = managementType;
            ViewBag.SearchTerm = searchTerm;
            
            return View(hostels);
        }

        // GET: Hostel/InstitutionManaged - Filter to show only institution-managed hostels
        public async Task<IActionResult> InstitutionManaged(string searchTerm = null)
        {
            IQueryable<Hostel> query = _context.Hostels.Include(h => h.Rooms)
                .Where(h => h.ManagementType == ManagementType.InstitutionManaged);
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(h => 
                    h.Name.ToLower().Contains(searchTerm) || 
                    h.Location.ToLower().Contains(searchTerm) || 
                    (h.HostelCode != null && h.HostelCode.ToLower().Contains(searchTerm)));
            }
            
            var hostels = await query.ToListAsync();
            
            ViewBag.ManagementType = ManagementType.InstitutionManaged;
            ViewBag.SearchTerm = searchTerm;
            
            return View("Index", hostels);
        }

        // GET: Hostel/PrivatelyManaged - Filter to show only privately-managed hostels
        public async Task<IActionResult> PrivatelyManaged(string searchTerm = null)
        {
            IQueryable<Hostel> query = _context.Hostels.Include(h => h.Rooms)
                .Where(h => h.ManagementType == ManagementType.PrivatelyManaged);
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(h => 
                    h.Name.ToLower().Contains(searchTerm) || 
                    h.Location.ToLower().Contains(searchTerm) || 
                    (h.HostelCode != null && h.HostelCode.ToLower().Contains(searchTerm)));
            }
            
            var hostels = await query.ToListAsync();
            
            ViewBag.ManagementType = ManagementType.PrivatelyManaged;
            ViewBag.SearchTerm = searchTerm;
            
            return View("Index", hostels);
        }
        
        // GET: Hostel/Gallery - Public gallery of hostels
        [AllowAnonymous]
        public async Task<IActionResult> Gallery(ManagementType? managementType = null)
        {
            IQueryable<Hostel> query = _context.Hostels.Where(h => h.IsActive);
            
            if (managementType.HasValue)
            {
                query = query.Where(h => h.ManagementType == managementType.Value);
            }
            
            var hostels = await query.ToListAsync();
            
            ViewBag.ManagementType = managementType;
            
            return View(hostels);
        }

        // GET: Hostel/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var hostel = await _context.Hostels
                .Include(h => h.Rooms)
                .Include(h => h.Amenities)
                .FirstOrDefaultAsync(h => h.HostelId == id);

            if (hostel == null)
            {
                return NotFound();
            }

            // Set a ViewBag flag to indicate if admin controls should be shown
            ViewBag.IsAdmin = User.Identity?.IsAuthenticated == true && 
                             (User.IsInRole("Admin") || User.IsInRole("Dean") || User.IsInRole("Warden"));

            // Group rooms by floor for layout visualization
            // Assuming first character of room number indicates floor (e.g., "1xx" is first floor)
            var roomsByFloor = hostel.Rooms
                .GroupBy(r => r.RoomNumber.Length > 0 ? r.RoomNumber.Substring(0, 1) : "0")
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.OrderBy(r => r.RoomNumber).ToList());

            ViewBag.RoomsByFloor = roomsByFloor;

            return View(hostel);
        }
        
        // GET: Hostel/RoomDetails/5
        [AllowAnonymous]
        public async Task<IActionResult> RoomDetails(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Hostel)
                .Include(r => r.OccupyingStudents)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            ViewBag.IsAdmin = User.Identity?.IsAuthenticated == true && 
                             (User.IsInRole("Admin") || User.IsInRole("Dean") || User.IsInRole("Warden"));

            return View(room);
        }

        // GET: Hostel/HostelInterior/5
        [AllowAnonymous]
        public async Task<IActionResult> HostelInterior(int id)
        {
            var hostel = await _context.Hostels
                .Include(h => h.Amenities)
                .FirstOrDefaultAsync(h => h.HostelId == id);

            if (hostel == null)
            {
                return NotFound();
            }

            return View(hostel);
        }

        // GET: Hostel/PublicDetails/5
        [AllowAnonymous]
        public async Task<IActionResult> PublicDetails(int id)
        {
            // Redirect to the main Details action which is now accessible to all
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Hostel/Create
        public IActionResult Create()
        {
            return View(new HostelViewModel());
        }

        // POST: Hostel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HostelViewModel model, List<IFormFile>? InteriorImages, List<string>? HostelServices, List<string>? AmenityNames, List<string>? AmenityDescriptions)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Generate hostel code based on management type
                        string prefix = model.ManagementType == ManagementType.InstitutionManaged ? "BUH" : "BUHA";
                        
                        // Find the highest existing code number for this prefix - FIXED NULL ISSUE
                        int nextNumber = 1; // Default starting number
                        var existingCodes = await _context.Hostels
                            .Where(h => h.HostelCode != null && h.HostelCode.StartsWith(prefix))
                            .Select(h => h.HostelCode)
                            .ToListAsync();
                            
                        if (existingCodes.Any())
                        {
                            // Extract the numeric parts and find the highest
                            var numbers = new List<int>();
                            
                            foreach (var code in existingCodes)
                            {
                                if (code.Length <= prefix.Length)
                                    continue;
                                    
                                string numPart = code.Substring(prefix.Length);
                                if (int.TryParse(numPart, out int num))
                                    numbers.Add(num);
                            }
                            
                            if (numbers.Any())
                            {
                                nextNumber = numbers.Max() + 1;
                            }
                        }
                        
                        // Format the code with leading zeros (e.g., BUH001, BUHA001)
                        string hostelCode = $"{prefix}{nextNumber:D3}";
                        
                        var hostel = new Hostel
                        {
                            HostelCode = hostelCode,
                            Name = model.Name,
                            Location = model.Location,
                            Description = model.Description ?? string.Empty,
                            Gender = model.Gender,
                            ManagementType = model.ManagementType,
                            Capacity = model.Capacity,
                            IsActive = model.IsActive,
                            WardenName = model.WardenName ?? string.Empty,
                            WardenContact = model.WardenContact ?? string.Empty,
                            DistanceFromCampus = model.DistanceFromCampus,
                            YouTubeVideoId = model.YouTubeVideoId // Added YouTubeVideoId
                        };

                        // Process main hostel image
                        if (model.ImageFile != null)
                        {
                            string? uniqueFileName = ProcessUploadedFile(model.ImageFile);
                            hostel.ImageUrl = uniqueFileName;
                        }

                        // Save the hostel to get its ID for related entities
                        _context.Add(hostel);
                        await _context.SaveChangesAsync();

                        // Process interior images (if any)
                        if (InteriorImages != null && InteriorImages.Count > 0)
                        {
                            try
                            {
                                // Create a HostelImage subdirectory with the hostel's id
                                string hostelImagesDir = Path.Combine(_hostingEnvironment.WebRootPath, "images", "hostels", hostel.HostelId.ToString());
                                if (!Directory.Exists(hostelImagesDir))
                                {
                                    Directory.CreateDirectory(hostelImagesDir);
                                }

                                // Define standard image names
                                string[] standardNames = { "room-interior.jpg", "dorm-room.jpg", "bathroom.jpg", "lounge.jpg" };

                                // Process each interior image
                                for (int i = 0; i < InteriorImages.Count && i < standardNames.Length; i++)
                                {
                                    var file = InteriorImages[i];
                                    if (file != null && file.Length > 0)
                                    {
                                        string filePath = Path.Combine(hostelImagesDir, standardNames[i]);
                                        
                                        // Save the file
                                        using (var stream = new FileStream(filePath, FileMode.Create))
                                        {
                                            await file.CopyToAsync(stream);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error processing interior images");
                                // Continue even if image processing fails
                            }
                        }

                        // Process amenities
                        if (AmenityNames != null && AmenityNames.Count > 0)
                        {
                            for (int i = 0; i < AmenityNames.Count; i++)
                            {
                                if (!string.IsNullOrWhiteSpace(AmenityNames[i]))
                                {
                                    var amenity = new Amenity
                                    {
                                        HostelId = hostel.HostelId,
                                        Name = AmenityNames[i],
                                        Description = (AmenityDescriptions != null && i < AmenityDescriptions.Count) 
                                            ? AmenityDescriptions[i] 
                                            : string.Empty,
                                        IconClass = GetIconClassForAmenity(AmenityNames[i]),
                                        IsActive = true
                                    };

                                    _context.Amenities.Add(amenity);
                                }
                            }
                        }

                        // Process services (these would typically be stored as amenities with a specific category)
                        if (HostelServices != null && HostelServices.Count > 0)
                        {
                            foreach (var service in HostelServices)
                            {
                                var amenity = new Amenity
                                {
                                    HostelId = hostel.HostelId,
                                    Name = service,
                                    Description = GetServiceDescription(service),
                                    IconClass = GetIconClassForService(service),
                                    IsActive = true
                                };

                                _context.Amenities.Add(amenity);
                            }
                        }

                        await _context.SaveChangesAsync();
                        
                        // Log the activity
                        TempData["SuccessMessage"] = $"Hostel '{hostel.Name}' was successfully created with all amenities and images.";
                        return RedirectToAction(nameof(Details), new { id = hostel.HostelId });
                    }
                    catch (Exception ex)
                    {
                        // Log the error
                        _logger.LogError(ex, "Error creating hostel");
                        ModelState.AddModelError("", $"Error creating hostel: {ex.Message}");
                        
                        // Check for inner exception which often contains more detailed DB errors
                        if (ex.InnerException != null)
                        {
                            _logger.LogError(ex.InnerException, "Inner exception details");
                            ModelState.AddModelError("", $"Details: {ex.InnerException.Message}");
                        }
                    }
                }
                else
                {
                    // Log validation errors
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            _logger.LogError($"Validation error for {state.Key}: {error.ErrorMessage}");
                            ModelState.AddModelError("", $"Validation error for {state.Key}: {error.ErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Catch any unexpected errors
                _logger.LogError(ex, "Unexpected error in Create action");
                ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
            }
            
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: Hostel/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var hostel = await _context.Hostels.FindAsync(id);
            if (hostel == null)
            {
                return NotFound();
            }

            var model = new HostelViewModel
            {
                HostelId = hostel.HostelId,
                HostelCode = hostel.HostelCode,
                Name = hostel.Name,
                Location = hostel.Location,
                Description = hostel.Description ?? string.Empty,
                Gender = hostel.Gender,
                ManagementType = hostel.ManagementType,
                Capacity = hostel.Capacity,
                IsActive = hostel.IsActive,
                WardenName = hostel.WardenName ?? string.Empty,
                WardenContact = hostel.WardenContact ?? string.Empty,
                DistanceFromCampus = hostel.DistanceFromCampus,
                ExistingImagePath = hostel.ImageUrl,
                YouTubeVideoId = hostel.YouTubeVideoId // Added YouTubeVideoId
            };

            return View(model);
        }

        // POST: Hostel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HostelViewModel model)
        {
            if (id != model.HostelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var hostel = await _context.Hostels.FindAsync(id);
                    if (hostel == null)
                    {
                        return NotFound();
                    }

                    hostel.Name = model.Name;
                    hostel.Location = model.Location;
                    hostel.Description = model.Description ?? string.Empty;
                    hostel.Gender = model.Gender;
                    hostel.ManagementType = model.ManagementType;
                    hostel.Capacity = model.Capacity;
                    hostel.IsActive = model.IsActive;
                    hostel.WardenName = model.WardenName ?? string.Empty;
                    hostel.WardenContact = model.WardenContact ?? string.Empty;
                    hostel.DistanceFromCampus = model.DistanceFromCampus;
                    hostel.YouTubeVideoId = model.YouTubeVideoId; // Added YouTubeVideoId

                    if (model.ImageFile != null)
                    {
                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(hostel.ImageUrl))
                        {
                            DeleteImage(hostel.ImageUrl);
                        }

                        // Save new image
                        string? uniqueFileName = ProcessUploadedFile(model.ImageFile);
                        hostel.ImageUrl = uniqueFileName;
                    }

                    _context.Update(hostel);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Hostel '{hostel.Name}' was successfully updated.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HostelExists(model.HostelId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    // Log the error
                    _logger.LogError(ex, "Error updating hostel");
                    ModelState.AddModelError("", $"Error updating hostel: {ex.Message}");
                }
            }
            return View(model);
        }

        // GET: Hostel/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var hostel = await _context.Hostels
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(m => m.HostelId == id);
                
            if (hostel == null)
            {
                return NotFound();
            }

            // Check if this hostel has students assigned
            bool hasAssignedStudents = await _context.Users.AnyAsync(u => u.CurrentHostelId == id);
            if (hasAssignedStudents)
            {
                TempData["ErrorMessage"] = "Cannot delete this hostel as it has students assigned to it.";
                return RedirectToAction(nameof(Index));
            }

            return View(hostel);
        }

        // POST: Hostel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hostel = await _context.Hostels
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.HostelId == id);
                
            if (hostel == null)
            {
                return NotFound();
            }

            // Check if this hostel has students assigned
            bool hasAssignedStudents = await _context.Users.AnyAsync(u => u.CurrentHostelId == id);
            if (hasAssignedStudents)
            {
                TempData["ErrorMessage"] = "Cannot delete this hostel as it has students assigned to it.";
                return RedirectToAction(nameof(Index));
            }

            // Delete the hostel image if it exists
            if (!string.IsNullOrEmpty(hostel.ImageUrl))
            {
                DeleteImage(hostel.ImageUrl);
            }

            // Remove all rooms first
            _context.Rooms.RemoveRange(hostel.Rooms);
            
            // Remove amenities
            var amenities = await _context.Amenities.Where(a => a.HostelId == id).ToListAsync();
            _context.Amenities.RemoveRange(amenities);
            
            // Remove announcements
            var announcements = await _context.Announcements.Where(a => a.HostelId == id).ToListAsync();
            _context.Announcements.RemoveRange(announcements);

            // Finally remove the hostel
            _context.Hostels.Remove(hostel);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = $"Hostel '{hostel.Name}' was successfully deleted.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Hostel/RoomLayout/5
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> RoomLayout(int id)
        {
            var hostel = await _context.Hostels
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.HostelId == id);

            if (hostel == null)
            {
                return NotFound();
            }

            ViewBag.HostelId = id;
            ViewBag.HostelName = hostel.Name;

            return View(hostel);
        }
        
        // GET: Hostel/AddRoom/5
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> AddRoom(int id)
        {
            var hostel = await _context.Hostels.FindAsync(id);
            if (hostel == null)
            {
                return NotFound();
            }

            var model = new RoomViewModel
            {
                HostelId = id,
                HostelName = hostel.Name ?? string.Empty
            };

            return View(model);
        }
        
        // POST: Hostel/AddRoom
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> AddRoom(RoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if room number already exists in this hostel
                    bool roomExists = await _context.Rooms
                        .AnyAsync(r => r.HostelId == model.HostelId && r.RoomNumber == model.RoomNumber);
                        
                    if (roomExists)
                    {
                        ModelState.AddModelError("RoomNumber", "This room number already exists in this hostel.");
                        var hostel = await _context.Hostels.FindAsync(model.HostelId);
                        model.HostelName = hostel?.Name ?? string.Empty;
                        return View(model);
                    }

                    var room = new Room
                    {
                        RoomNumber = model.RoomNumber,
                        HostelId = model.HostelId,
                        Type = model.Type,
                        Capacity = model.Capacity,
                        Description = model.Description ?? string.Empty,
                        PricePerSemester = model.PricePerSemester,
                        Status = RoomStatus.Available,
                        CurrentOccupancy = 0
                    };

                    _context.Add(room);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Room {model.RoomNumber} was successfully added to hostel.";
                    return RedirectToAction(nameof(RoomLayout), new { id = model.HostelId });
                }
                catch (Exception ex)
                {
                    // Log the error
                    _logger.LogError(ex, "Error adding room");
                    ModelState.AddModelError("", $"Error adding room: {ex.Message}");
                }
            }

            // If we got this far, something failed, redisplay form
            var hostelName = await _context.Hostels
                .Where(h => h.HostelId == model.HostelId)
                .Select(h => h.Name)
                .FirstOrDefaultAsync();
                
            model.HostelName = hostelName ?? string.Empty;
            return View(model);
        }

        // GET: Hostel/AddAmenity/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAmenity(int id)
        {
            var hostel = await _context.Hostels.FindAsync(id);
            if (hostel == null)
            {
                return NotFound();
            }

            var model = new AmenityViewModel
            {
                HostelId = id,
                HostelName = hostel.Name ?? string.Empty
            };

            return View(model);
        }
        
        // POST: Hostel/AddAmenity
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAmenity(AmenityViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var amenity = new Amenity
                    {
                        HostelId = model.HostelId,
                        Name = model.Name,
                        Description = model.Description ?? string.Empty,
                        IconClass = model.IconClass ?? string.Empty,
                        IsActive = model.IsActive
                    };

                    _context.Add(amenity);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Amenity '{model.Name}' was successfully added to hostel.";
                    return RedirectToAction(nameof(Details), new { id = model.HostelId });
                }
                catch (Exception ex)
                {
                    // Log the error
                    _logger.LogError(ex, "Error adding amenity");
                    ModelState.AddModelError("", $"Error adding amenity: {ex.Message}");
                }
            }

            // If we got this far, something failed, redisplay form
            var hostelName = await _context.Hostels
                .Where(h => h.HostelId == model.HostelId)
                .Select(h => h.Name)
                .FirstOrDefaultAsync();
                
            model.HostelName = hostelName ?? string.Empty;
            return View(model);
        }

        // Helper methods
        private bool HostelExists(int id)
        {
            return _context.Hostels.Any(e => e.HostelId == id);
        }

        private string? ProcessUploadedFile(IFormFile file)
        {
            if (file == null)
            {
                return null;
            }

            string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "hostels");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            // Return the relative path for storage in the database
            return "/images/hostels/" + uniqueFileName;
        }

        private void DeleteImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return;
            }
            
            string fullPath = Path.Combine(_hostingEnvironment.WebRootPath, imagePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
        
        // Helper methods for amenities and services
        private string GetIconClassForAmenity(string amenityName)
        {
            // Map common amenity names to Font Awesome icons
            return amenityName.ToLower() switch
            {
                var n when n.Contains("study") => "fas fa-book",
                var n when n.Contains("gym") => "fas fa-dumbbell",
                var n when n.Contains("laundry") => "fas fa-tshirt",
                var n when n.Contains("tv") || n.Contains("television") => "fas fa-tv",
                var n when n.Contains("lounge") => "fas fa-couch",
                var n when n.Contains("kitchen") => "fas fa-utensils",
                var n when n.Contains("parking") => "fas fa-parking",
                _ => "fas fa-check-circle" // Default icon
            };
        }

        private string GetIconClassForService(string serviceName)
        {
            // Map service names to Font Awesome icons
            return serviceName.ToLower() switch
            {
                "wifi" => "fas fa-wifi",
                "cleaning" => "fas fa-broom",
                "security" => "fas fa-shield-alt",
                "kitchen" => "fas fa-utensils",
                _ => "fas fa-concierge-bell" // Default service icon
            };
        }

        private string GetServiceDescription(string serviceName)
        {
            // Return descriptions for common services
            return serviceName.ToLower() switch
            {
                "wifi" => "High-speed wireless internet access throughout the building.",
                "cleaning" => "Regular cleaning services for common areas and optional room cleaning.",
                "security" => "24/7 security personnel and CCTV surveillance for student safety.",
                "kitchen" => "Shared kitchen facilities with refrigerator and basic cooking equipment.",
                _ => $"{serviceName} service available to all residents."
            };
        }
    }
}