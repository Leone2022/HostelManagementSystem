using HostelMS.Models;
using System.ComponentModel.DataAnnotations;

namespace HostelMS.ViewModels
{
    public class RoomViewModel
    {
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Room number is required")]
        [StringLength(20, ErrorMessage = "Room number cannot be longer than 20 characters")]
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hostel selection is required")]
        [Display(Name = "Hostel")]
        public int HostelId { get; set; }
        
        [Display(Name = "Hostel Name")]
        public string HostelName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Room type is required")]
        [Display(Name = "Room Type")]
        public RoomType Type { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20")]
        [Display(Name = "Capacity (Beds)")]
        public int Capacity { get; set; }

        [Display(Name = "Description")]
        [StringLength(200, ErrorMessage = "Description cannot be longer than 200 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price per semester is required")]
        [Range(1, 1000000, ErrorMessage = "Price must be between 1 and 1,000,000")]
        [Display(Name = "Price Per Semester")]
        public decimal PricePerSemester { get; set; }

        [Display(Name = "Status")]
        public RoomStatus Status { get; set; } = RoomStatus.Available;

        [Display(Name = "Current Occupancy")]
        public int CurrentOccupancy { get; set; } = 0;

        [Display(Name = "Needs Maintenance")]
        public bool NeedsAttention { get; set; } = false;
    }
}