using HostelMS.Models;
using System;
using System.Collections.Generic;

namespace HostelMS.ViewModels
{
    public class WardenDashboardViewModel
    {
        public int TotalAssignedStudents { get; set; }
        public int PendingVerifications { get; set; }
        public int AvailableBeds { get; set; }
        public int MaintenanceRequests { get; set; }
        public List<Hostel> Hostels { get; set; } = new List<Hostel>();
        public List<ActivityViewModel> RecentActivities { get; set; } = new List<ActivityViewModel>();
        public List<ApplicationUser>? SearchResults { get; set; }
    }

    public class ActivityViewModel
    {
        public string ActivityType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}