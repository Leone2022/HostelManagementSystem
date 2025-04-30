using HostelMS.Models;
using System.Collections.Generic;

namespace HostelMS.ViewModels
{
    public class StudentSearchViewModel
    {
        public string? SearchTerm { get; set; }
        public int? HostelId { get; set; }
        public bool? VerificationStatus { get; set; }
        public bool? ApprovalStatus { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public List<Hostel>? Hostels { get; set; }
        public List<ApplicationUser>? Students { get; set; }
    }
}