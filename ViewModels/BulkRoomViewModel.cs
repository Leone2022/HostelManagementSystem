using HostelMS.Models;
using System.ComponentModel.DataAnnotations;

namespace HostelMS.ViewModels
{
    public class BulkRoomViewModel
    {
        public int HostelId { get; set; }
        public string HostelName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Prefix is required")]
        [Display(Name = "Room Number Prefix")]
        public string RoomNumberPrefix { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Starting number is required")]
        [Range(1, 1000, ErrorMessage = "Starting number must be between 1 and 1000")]
        [Display(Name = "Starting Number")]
        public int StartingNumber { get; set; } = 1;
        
        [Required(ErrorMessage = "Number of rooms is required")]
        [Range(1, 100, ErrorMessage = "You can create between 1 and 100 rooms at once")]
        [Display(Name = "Number of Rooms to Create")]
        public int NumberOfRooms { get; set; } = 1;
        
        [Required(ErrorMessage = "Room type is required")]
        [Display(Name = "Room Type")]
        public RoomType Type { get; set; }
        
        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20")]
        [Display(Name = "Capacity (Beds)")]
        public int Capacity { get; set; }
        
        [Display(Name = "Description")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Price per semester is required")]
        [Range(1, 1000000, ErrorMessage = "Price must be between 1 and 1,000,000")]
        [Display(Name = "Price Per Semester")]
        public decimal PricePerSemester { get; set; }
    }
}