using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HostelMS.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Hostel> Hostels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<StudentActivity> StudentActivities { get; set; }
        public DbSet<Notification> Notifications { get; set; } = null!;
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Configure Hostel entity
            builder.Entity<Hostel>()
                .Property(h => h.YouTubeVideoId)
                .IsRequired(false); // Make YouTubeVideoId nullable
                
            // Add any other model configurations or seed data here
        }
        
        // Remove the OnConfiguring method entirely - it's causing the error
    }
}