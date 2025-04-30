using HostelMS.Models;
using System.Collections.Generic;

namespace HostelMS.ViewModels
{
    public class StudentDashboardViewModel
    {
        // Student information
        public ApplicationUser Student { get; set; } = new ApplicationUser();
        
        // Room and hostel information 
        public Room? CurrentRoom { get; set; }
        public Hostel? CurrentHostel { get; set; }
        
        // Booking information
        public Booking? ActiveBooking { get; set; }
        public bool HasPendingBooking { get; set; }
        public bool HasApprovedBooking { get; set; }
        
        // Notifications
        public List<Notification>? RecentNotifications { get; set; }
        
        // Available hostels for booking
        public List<Hostel>? AvailableHostels { get; set; }
    }
}