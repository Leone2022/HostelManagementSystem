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
using System.Text;
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
        public async Task<IActionResult> Create(
            HostelViewModel model,
            List<IFormFile>? InteriorImages,
            List<string>? HostelServices,
            List<string>? AmenityNames,
            List<string>? AmenityDescriptions,
            List<string>? AvailableRoomTypes,
            decimal? SingleRoomPrice,
            decimal? DoubleRoomPrice,
            decimal? TripleRoomPrice,
            decimal? DormitoryPrice)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Generate hostel code based on management type
                        string prefix = model.ManagementType == ManagementType.InstitutionManaged ? "BUH" : "BUHA";

                        // Find the highest existing code number for this prefix
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
                            Gender = (Gender)model.Gender,
                            ManagementType = model.ManagementType,
                            Capacity = model.Capacity,
                            IsActive = model.IsActive,
                            WardenName = model.WardenName ?? string.Empty,
                            WardenContact = model.WardenContact ?? string.Empty,
                            DistanceFromCampus = model.DistanceFromCampus,
                            YouTubeVideoId = model.YouTubeVideoId,
                            MinPrice = new[] { SingleRoomPrice, DoubleRoomPrice, TripleRoomPrice, DormitoryPrice }
                                .Where(p => p.HasValue && p > 0)
                                .Min(),
                            MaxPrice = new[] { SingleRoomPrice, DoubleRoomPrice, TripleRoomPrice, DormitoryPrice }
                                .Where(p => p.HasValue && p > 0)
                                .Max(),
                            AvailableRoomTypes = AvailableRoomTypes != null ? string.Join(",", AvailableRoomTypes) : null
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

                        // Create quick rooms if enabled
                        if (model.EnableQuickSetup && model.FloorCount.HasValue && model.RoomsPerFloor.HasValue)
                        {
                            await CreateQuickRooms(hostel.HostelId, model, SingleRoomPrice, DoubleRoomPrice, TripleRoomPrice, DormitoryPrice);
                        }

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
                Gender = (Gender)hostel.Gender,
                ManagementType = hostel.ManagementType,
                Capacity = hostel.Capacity,
                IsActive = hostel.IsActive,
                WardenName = hostel.WardenName ?? string.Empty,
                WardenContact = hostel.WardenContact ?? string.Empty,
                DistanceFromCampus = hostel.DistanceFromCampus,
                ExistingImagePath = hostel.ImageUrl,
                YouTubeVideoId = hostel.YouTubeVideoId
            };

            return View(model);
        }

        // POST: Hostel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HostelViewModel model,
                                                List<string>? HostelServices,
                                                List<string>? AmenityNames,
                                                List<string>? AmenityDescriptions)
        {
            if (id != model.HostelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var hostel = await _context.Hostels
                                    .Include(h => h.Amenities) // Include amenities to update them
                                    .FirstOrDefaultAsync(h => h.HostelId == id);

                    if (hostel == null)
                    {
                        return NotFound();
                    }

                    hostel.Name = model.Name;
                    hostel.Location = model.Location;
                    hostel.Description = model.Description ?? string.Empty;
                    hostel.Gender = (Gender)model.Gender;
                    hostel.ManagementType = model.ManagementType;
                    hostel.Capacity = model.Capacity;
                    hostel.IsActive = model.IsActive;
                    hostel.WardenName = model.WardenName ?? string.Empty;
                    hostel.WardenContact = model.WardenContact ?? string.Empty;
                    hostel.DistanceFromCampus = model.DistanceFromCampus;
                    hostel.YouTubeVideoId = model.YouTubeVideoId;

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

                    // --- Update Amenities and Services ---
                    // Clear existing amenities and services first
                    _context.Amenities.RemoveRange(hostel.Amenities);
                    await _context.SaveChangesAsync(); // Save changes to remove old entries

                    // Add amenities
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

                    // Add services
                    if (HostelServices != null && HostelServices.Count > 0)
                    {
                        foreach (var service in HostelServices)
                        {
                            var serviceAmenity = new Amenity
                            {
                                HostelId = hostel.HostelId,
                                Name = service,
                                Description = GetServiceDescription(service),
                                IconClass = GetIconClassForService(service),
                                IsActive = true
                            };
                            _context.Amenities.Add(serviceAmenity);
                        }
                    }
                    // --- End Update Amenities and Services ---

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

        [HttpGet]
        public async Task<IActionResult> OccupancyReport()
        {
            var hostels = await _context.Hostels
                .Include(h => h.Rooms)
                .OrderBy(h => h.Name)
                .ToListAsync();

            return View(hostels);
        }

        [HttpGet]
        public async Task<IActionResult> GenerateOccupancyPDF()
        {
            var hostels = await _context.Hostels
                .Include(h => h.Rooms)
                .OrderBy(h => h.Name)
                .ToListAsync();

            var reportHtml = GenerateOccupancyReportHtml(hostels);
            return Content(reportHtml, "text/html");
        }

        [HttpGet]
        public async Task<IActionResult> ExportOccupancyCSV()
        {
            var hostels = await _context.Hostels
                .Include(h => h.Rooms)
                .OrderBy(h => h.Name)
                .ToListAsync();

            var csv = GenerateOccupancyCSV(hostels);
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"Hostel_Occupancy_Report_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }

        private string GenerateOccupancyReportHtml(List<Hostel> hostels)
        {
            var totalCapacity = hostels.Sum(h => h.Rooms.Sum(r => r.Capacity));
            var totalOccupancy = hostels.Sum(h => h.Rooms.Sum(r => r.CurrentOccupancy));
            var overallOccupancyRate = totalCapacity > 0 ? (double)totalOccupancy / totalCapacity * 100 : 0;
            
            var activeHostels = hostels.Count(h => h.IsActive);
            var maleHostels = hostels.Count(h => h.Gender == Gender.Male);
            var femaleHostels = hostels.Count(h => h.Gender == Gender.Female);
            var institutionManaged = hostels.Count(h => h.ManagementType == ManagementType.InstitutionManaged);
            var privatelyManaged = hostels.Count(h => h.ManagementType == ManagementType.PrivatelyManaged);

            return $@"
<!DOCTYPE html>
<html>
<head>
    <title>Hostel Occupancy & Capacity Report</title>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 20px; background: #f8f9fa; }}
        .report-container {{ max-width: 1200px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 0 20px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; border-bottom: 3px solid #007bff; padding-bottom: 20px; margin-bottom: 30px; }}
        .header h1 {{ color: #007bff; margin: 0; font-size: 2.5em; }}
        .header h2 {{ color: #6c757d; margin: 5px 0 0 0; font-weight: normal; }}
        .report-info {{ background: #e3f2fd; padding: 15px; border-radius: 8px; margin-bottom: 30px; }}
        .summary-cards {{ display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 20px; margin-bottom: 30px; }}
        .summary-card {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 10px; text-align: center; }}
        .summary-card h3 {{ margin: 0 0 10px 0; font-size: 2.5em; }}
        .summary-card p {{ margin: 0; font-size: 1.1em; opacity: 0.9; }}
        .occupancy-overview {{ display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin-bottom: 30px; }}
        .occupancy-item {{ background: #f8f9fa; padding: 15px; border-radius: 8px; border-left: 4px solid #007bff; }}
        .occupancy-item h4 {{ margin: 0 0 10px 0; color: #495057; }}
        .progress-container {{ height: 20px; background: #e9ecef; border-radius: 10px; margin-bottom: 5px; }}
        .progress-bar {{ height: 100%; border-radius: 10px; background: linear-gradient(90deg, #4facfe 0%, #00f2fe 100%); }}
        .hostel-table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
        .hostel-table th, .hostel-table td {{ padding: 12px 15px; text-align: left; border-bottom: 1px solid #dee2e6; }}
        .hostel-table th {{ background-color: #007bff; color: white; }}
        .hostel-table tr:nth-child(even) {{ background-color: #f8f9fa; }}
        .hostel-table tr:hover {{ background-color: #e9ecef; }}
        .text-right {{ text-align: right; }}
        .text-center {{ text-align: center; }}
        .footer {{ margin-top: 30px; text-align: center; color: #6c757d; font-size: 0.9em; }}
    </style>
</head>
<body>
    <div class=""report-container"">
        <div class=""header"">
            <h1>Hostel Occupancy & Capacity Report</h1>
            <h2>Generated on {DateTime.Now.ToString("MMMM dd, yyyy")}</h2>
        </div>

        <div class=""report-info"">
            <p>This report provides a comprehensive overview of hostel occupancy rates, capacity utilization, and key statistics across all hostels in the system.</p>
        </div>

        <div class=""summary-cards"">
            <div class=""summary-card"">
                <h3>{totalOccupancy}/{totalCapacity}</h3>
                <p>Total Occupancy</p>
            </div>
            <div class=""summary-card"">
                <h3>{overallOccupancyRate:F1}%</h3>
                <p>Overall Occupancy Rate</p>
            </div>
            <div class=""summary-card"">
                <h3>{activeHostels}</h3>
                <p>Active Hostels</p>
            </div>
            <div class=""summary-card"">
                <h3>{hostels.Count}</h3>
                <p>Total Hostels</p>
            </div>
        </div>

        <div class=""occupancy-overview"">
            <div class=""occupancy-item"">
                <h4>Male Hostels</h4>
                <div class=""progress-container"">
                    <div class=""progress-bar"" style=""width: {(maleHostels * 100.0 / hostels.Count):F1}%""></div>
                </div>
                <p>{maleHostels} hostels ({maleHostels * 100.0 / hostels.Count:F1}%)</p>
            </div>
            <div class=""occupancy-item"">
                <h4>Female Hostels</h4>
                <div class=""progress-container"">
                    <div class=""progress-bar"" style=""width: {(femaleHostels * 100.0 / hostels.Count):F1}%""></div>
                </div>
                <p>{femaleHostels} hostels ({femaleHostels * 100.0 / hostels.Count:F1}%)</p>
            </div>
            <div class=""occupancy-item"">
                <h4>Institution Managed</h4>
                <div class=""progress-container"">
                    <div class=""progress-bar"" style=""width: {(institutionManaged * 100.0 / hostels.Count):F1}%""></div>
                </div>
                <p>{institutionManaged} hostels ({institutionManaged * 100.0 / hostels.Count:F1}%)</p>
            </div>
            <div class=""occupancy-item"">
                <h4>Privately Managed</h4>
                <div class=""progress-container"">
                    <div class=""progress-bar"" style=""width: {(privatelyManaged * 100.0 / hostels.Count):F1}%""></div>
                </div>
                <p>{privatelyManaged} hostels ({privatelyManaged * 100.0 / hostels.Count:F1}%)</p>
            </div>
        </div>

        <table class=""hostel-table"">
            <thead>
                <tr>
                    <th>Hostel</th>
                    <th>Type</th>
                    <th>Management</th>
                    <th>Rooms</th>
                    <th>Capacity</th>
                    <th>Occupied</th>
                    <th>Occupancy Rate</th>
                </tr>
            </thead>
            <tbody>
                {string.Join("", hostels.Select(h => {
                    var capacity = h.Rooms.Sum(r => r.Capacity);
                    var occupied = h.Rooms.Sum(r => r.CurrentOccupancy);
                    var rate = capacity > 0 ? (occupied * 100.0 / capacity) : 0;
                    return $@"
                <tr>
                    <td>{h.Name}</td>
                    <td>{h.Gender}</td>
                    <td>{h.ManagementType}</td>
                    <td class=""text-right"">{h.Rooms.Count}</td>
                    <td class=""text-right"">{capacity}</td>
                    <td class=""text-right"">{occupied}</td>
                    <td>
                        <div class=""progress-container"">
                            <div class=""progress-bar"" style=""width: {rate:F1}%""></div>
                        </div>
                        <span>{rate:F1}%</span>
                    </td>
                </tr>";
                }))}
            </tbody>
        </table>

        <div class=""footer"">
            <p>Report generated by Hostel Management System &copy; {DateTime.Now.Year}</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GenerateOccupancyCSV(List<Hostel> hostels)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Hostel Name,Gender,Management Type,Room Count,Total Capacity,Current Occupancy,Occupancy Rate");

            foreach (var hostel in hostels)
            {
                var capacity = hostel.Rooms.Sum(r => r.Capacity);
                var occupied = hostel.Rooms.Sum(r => r.CurrentOccupancy);
                var rate = capacity > 0 ? (occupied * 100.0 / capacity) : 0;

                csv.AppendLine($"\"{hostel.Name}\",{hostel.Gender},{hostel.ManagementType},{hostel.Rooms.Count},{capacity},{occupied},{rate:F1}%");
            }

            // Add summary data
            var totalCapacity = hostels.Sum(h => h.Rooms.Sum(r => r.Capacity));
            var totalOccupancy = hostels.Sum(h => h.Rooms.Sum(r => r.CurrentOccupancy));
            var overallRate = totalCapacity > 0 ? (totalOccupancy * 100.0 / totalCapacity) : 0;

            csv.AppendLine();
            csv.AppendLine("SUMMARY DATA");
            csv.AppendLine($"Total Hostels,{hostels.Count}");
            csv.AppendLine($"Total Capacity,{totalCapacity}");
            csv.AppendLine($"Total Occupancy,{totalOccupancy}");
            csv.AppendLine($"Overall Occupancy Rate,{overallRate:F1}%");
            csv.AppendLine($"Male Hostels,{hostels.Count(h => h.Gender == Gender.Male)}");
            csv.AppendLine($"Female Hostels,{hostels.Count(h => h.Gender == Gender.Female)}");
            csv.AppendLine($"Institution Managed,{hostels.Count(h => h.ManagementType == ManagementType.InstitutionManaged)}");
            csv.AppendLine($"Privately Managed,{hostels.Count(h => h.ManagementType == ManagementType.PrivatelyManaged)}");

            return csv.ToString();
        }

        // Helper methods
        private string? ProcessUploadedFile(IFormFile? file)
{
    if (file == null)
    {
        return null;
    }

    string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "hostels");
    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

    Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists

    using (var fileStream = new FileStream(filePath, FileMode.Create))
    {
        file.CopyTo(fileStream);
    }
    
    // ✅ Return the web-accessible path, not just the filename
    return "/images/hostels/" + uniqueFileName;
}
        private void DeleteImage(string fileName)
        {
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", "hostels", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        private bool HostelExists(int id)
        {
            return _context.Hostels.Any(e => e.HostelId == id);
        }

        // Helper method to get amenity description (adjust logic as needed)
        private string GetAmenityDescription(string amenityName)
        {
            return amenityName.ToLower() switch
            {
                "pool" => "Swimming pool available for guests.",
                "gym" => "Fitness center with modern equipment.",
                "bar" => "On-site bar serving drinks and snacks.",
                "restaurant" => "Restaurant offering breakfast, lunch, and dinner.",
                "lounge" => "Common lounge area for relaxation.",
                "air conditioning" => "Rooms equipped with air conditioning.",
                "heating" => "Rooms equipped with heating.",
                "private bathroom" => "Private bathroom in each room.",
                "shared bathroom" => "Shared bathroom facilities.",
                "balcony" => "Rooms with private balconies.",
                "terrace" => "Outdoor terrace area.",
                "garden" => "Beautiful garden area.",
                "lockers" => "Secure lockers for personal belongings.",
                "bed linen" => "Fresh bed linen provided.",
                "towels" => "Towels provided.",
                _ => "General amenity description."
            };
        }

        // Helper method to get amenity icon class (adjust logic as needed)
        private string GetIconClassForAmenity(string amenityName)
        {
            return amenityName.ToLower() switch
            {
                "pool" => "fas fa-swimming-pool",
                "gym" => "fas fa-dumbbell",
                "bar" => "fas fa-cocktail",
                "restaurant" => "fas fa-utensils",
                "lounge" => "fas fa-couch",
                "air conditioning" => "fas fa-fan",
                "heating" => "fas fa-thermometer-half",
                "private bathroom" => "fas fa-bath",
                "shared bathroom" => "fas fa-toilet",
                "balcony" => "fas fa-building",
                "terrace" => "fas fa-sun",
                "garden" => "fas fa-leaf",
                "lockers" => "fas fa-lock",
                "bed linen" => "fas fa-bed",
                "towels" => "fas fa-hand-sparkles",
                _ => "fas fa-info-circle" // Default icon
            };
        }

        // Helper method to get service description
        private string GetServiceDescription(string serviceName)
        {
            return serviceName.ToLower() switch
            {
                "wifi" => "Free high-speed Wi-Fi available throughout the hostel.",
                "cleaning" => "Daily cleaning service for common areas and rooms.",
                "security" => "24/7 security personnel and CCTV surveillance.",
                "kitchen" => "Fully equipped communal kitchen for guest use.",
                "laundry" => "Self-service laundry facilities available.",
                "parking" => "On-site parking available for guests.",
                "maintenance" => "On-call maintenance for any issues.",
                "24/7 security" => "Round-the-clock security surveillance and personnel.",
                "cleaning service" => "Professional cleaning services for all areas.",
                _ => "General service description."
            };
        }

        // Helper method to get service icon class
        private string GetIconClassForService(string serviceName)
        {
            return serviceName.ToLower() switch
            {
                "wifi" => "fas fa-wifi",
                "cleaning" => "fas fa-broom",
                "security" => "fas fa-shield-alt",
                "kitchen" => "fas fa-utensils",
                "laundry" => "fas fa-tshirt",
                "parking" => "fas fa-parking",
                "maintenance" => "fas fa-tools",
                "24/7 security" => "fas fa-shield-alt",
                "cleaning service" => "fas fa-broom",
                _ => "fas fa-concierge-bell" // Default service icon
            };
        }

        // Private helper method to create quick rooms
        private async Task CreateQuickRooms(int hostelId, HostelViewModel model,
                             decimal? singlePrice, decimal? doublePrice,
                             decimal? triplePrice, decimal? dormitoryPrice)
        {
            if (!model.FloorCount.HasValue || !model.RoomsPerFloor.HasValue) return;

            for (int floor = 1; floor <= model.FloorCount.Value; floor++)
            {
                for (int roomNum = 1; roomNum <= model.RoomsPerFloor.Value; roomNum++)
                {
                    string roomNumber = $"{floor}{roomNum:D2}"; // e.g., "101", "102", "201"
                    RoomType type = RoomType.Double; // Default
                    decimal price = 500000; // Default price

                    // Determine room type and price based on model settings
                    if (model.DefaultRoomType.HasValue)
                    {
                        if (model.DefaultRoomType == RoomType.Single && singlePrice.HasValue)
                        {
                            type = RoomType.Single;
                            price = singlePrice.Value;
                        }
                        else if (model.DefaultRoomType == RoomType.Double && doublePrice.HasValue)
                        {
                            type = RoomType.Double;
                            price = doublePrice.Value;
                        }
                        else if (model.DefaultRoomType == RoomType.Triple && triplePrice.HasValue)
                        {
                            type = RoomType.Triple;
                            price = triplePrice.Value;
                        }
                        else if (model.DefaultRoomType == RoomType.Dormitory && dormitoryPrice.HasValue)
                        {
                            type = RoomType.Dormitory;
                            price = dormitoryPrice.Value;
                        }
                    }

                    var room = new Room
                    {
                        RoomNumber = roomNumber,
                        HostelId = hostelId,
                        Type = type,
                        Capacity = model.DefaultCapacity ?? 2,
                        Description = $"Room {roomNumber} on floor {floor}",
                        PricePerSemester = price,
                        Status = RoomStatus.Available,
                        CurrentOccupancy = 0
                    };

                    _context.Rooms.Add(room);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}