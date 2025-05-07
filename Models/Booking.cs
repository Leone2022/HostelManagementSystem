using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostelMS.Models
{
    public class Booking
    {
        public Booking()
        {
            // Initialize collections - will uncomment once all classes are defined
            // Payments = new HashSet<Payment>();
        }

        [Key]
        public int BookingId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public string? Comments { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public string? RejectionReason { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser? Student { get; set; }

        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set; }

        // public virtual ICollection<Payment> Payments { get; set; }
    }

    public enum BookingStatus
    {
        Pending,
        Approved,
        Rejected,
        CheckedIn,
        CheckedOut,
        Cancelled
    }
}