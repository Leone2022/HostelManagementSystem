using HostelMS.Models;
using System;
using System.Collections.Generic;

namespace HostelMS.ViewModels
{
    public class WardenDashboardViewModel
    {
        // Dashboard Statistics
        public int TotalAssignedStudents { get; set; }
        public int PendingVerifications { get; set; }
        public int AvailableBeds { get; set; }
        public int MaintenanceRequests { get; set; }
        public int TotalRooms { get; set; }
        public int OccupiedRooms { get; set; }
        
        // Occupancy Information
        public int TotalCapacity { get; set; }
        public int TotalOccupancy { get; set; }
        public double OccupancyRate { get; set; }
        
        // Collections
        public List<Hostel> Hostels { get; set; } = new List<Hostel>();
        public List<ActivityViewModel> RecentActivities { get; set; } = new List<ActivityViewModel>();
        public List<ApplicationUser>? SearchResults { get; set; }
        public IEnumerable<ApplicationUser> AllStudents { get; set; } = new List<ApplicationUser>();
        public List<Booking> RecentBookings { get; set; } = new List<Booking>();
        public List<MaintenanceRequest> PendingMaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
        
        // Current Hostel Information
        public Hostel? CurrentHostel { get; set; }
        public bool HasAssignedHostel => CurrentHostel != null;
    }

    public class ActivityViewModel
    {
        public string ActivityType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}