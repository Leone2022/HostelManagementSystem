using HostelMS.Models;
using System.ComponentModel.DataAnnotations;

namespace HostelMS.ViewModels
{
    public class BookingProcessViewModel
    {
        public int BookingId { get; set; }
        
        public Booking? Booking { get; set; }
        
        public Payment? Payment { get; set; }
        
        [Display(Name = "Approve Booking")]
        public bool IsApproved { get; set; }
        
        [Display(Name = "Rejection Reason")]
        [Required(ErrorMessage = "Please provide a reason for rejection", AllowEmptyStrings = false)]
        public string? RejectionReason { get; set; }
    }
}