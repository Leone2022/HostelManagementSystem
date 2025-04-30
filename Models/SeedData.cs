using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HostelMS.Models
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Check if we already have users
                if (context.Users.Any())
                {
                    return; // Database has been seeded
                }

                // Create roles
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                
                string[] roles = { "Admin", "Dean", "Warden", "Student" };
                
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
                
                // Create default admin user
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@bugema.ac.ug",
                    Email = "admin@bugema.ac.ug",
                    FirstName = "System",
                    LastName = "Administrator",
                    UserRole = "Admin",
                    EmailConfirmed = true
                };
                
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                
                // Create default dean user
                var deanUser = new ApplicationUser
                {
                    UserName = "dean@bugema.ac.ug",
                    Email = "dean@bugema.ac.ug",
                    FirstName = "Dean",
                    LastName = "Students",
                    UserRole = "Dean",
                    EmailConfirmed = true
                };
                
                result = await userManager.CreateAsync(deanUser, "Dean@123");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(deanUser, "Dean");
                }
                
                // Create default warden user
                var wardenUser = new ApplicationUser
                {
                    UserName = "warden@bugema.ac.ug",
                    Email = "warden@bugema.ac.ug",
                    FirstName = "Hostel",
                    LastName = "Warden",
                    UserRole = "Warden",
                    EmailConfirmed = true
                };
                
                result = await userManager.CreateAsync(wardenUser, "Warden@123");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(wardenUser, "Warden");
                }
                
                // Create sample student user
                var studentUser = new ApplicationUser
                {
                    UserName = "student@bugema.ac.ug",
                    Email = "student@bugema.ac.ug",
                    FirstName = "Sample",
                    LastName = "Student",
                    UserRole = "Student",
                    StudentId = "BUG-2023-001",
                    Course = "Computer Science",
                    Year = "3",
                    EmailConfirmed = true
                };
                
                result = await userManager.CreateAsync(studentUser, "Student@123");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(studentUser, "Student");
                }
                
                // Create sample hostels
                var maleHostel = new Hostel
                {
                    Name = "Male Hostel A",
                    Location = "North Campus",
                    Description = "A comfortable hostel for male students with modern amenities.",
                    Gender = Gender.Male,
                    Capacity = 200,
                    IsActive = true,
                    WardenName = "Mr. John Doe",
                    WardenContact = "+256 700 123 456",
                    DistanceFromCampus = 0.2m
                };
                
                var femaleHostel = new Hostel
                {
                    Name = "Female Hostel B",
                    Location = "South Campus",
                    Description = "A secure and comfortable hostel for female students.",
                    Gender = Gender.Female,
                    Capacity = 180,
                    IsActive = true,
                    WardenName = "Mrs. Jane Smith",
                    WardenContact = "+256 700 654 321",
                    DistanceFromCampus = 0.3m
                };
                
                context.Hostels.Add(maleHostel);
                context.Hostels.Add(femaleHostel);
                await context.SaveChangesAsync();
                
                // Create sample rooms
                var room1 = new Room
                {
                    RoomNumber = "A101",
                    HostelId = maleHostel.HostelId,
                    Type = RoomType.Double,
                    Capacity = 2,
                    Description = "Standard double room with two beds and study desks.",
                    PricePerSemester = 500000,
                    Status = RoomStatus.Available
                };
                
                var room2 = new Room
                {
                    RoomNumber = "A102",
                    HostelId = maleHostel.HostelId,
                    Type = RoomType.Single,
                    Capacity = 1,
                    Description = "Premium single room with private bathroom.",
                    PricePerSemester = 800000,
                    Status = RoomStatus.Available
                };
                
                var room3 = new Room
                {
                    RoomNumber = "B101",
                    HostelId = femaleHostel.HostelId,
                    Type = RoomType.Double,
                    Capacity = 2,
                    Description = "Standard double room with two beds and study desks.",
                    PricePerSemester = 500000,
                    Status = RoomStatus.Available
                };
                
                var room4 = new Room
                {
                    RoomNumber = "B102",
                    HostelId = femaleHostel.HostelId,
                    Type = RoomType.Single,
                    Capacity = 1,
                    Description = "Premium single room with private bathroom.",
                    PricePerSemester = 800000,
                    Status = RoomStatus.Available
                };
                
                context.Rooms.Add(room1);
                context.Rooms.Add(room2);
                context.Rooms.Add(room3);
                context.Rooms.Add(room4);
                await context.SaveChangesAsync();
            }
        }
    }
}