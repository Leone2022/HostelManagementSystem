using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostelMS.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        
        [Required]
        public int BookingId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        
        [Required]
        public PaymentMethod Method { get; set; }
        
        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        
        public string? TransactionReference { get; set; }
        
        public string? ReceiptNumber { get; set; }
        
        public string? Notes { get; set; }
        
        // New field for proof of payment image/file
        public string? ProofOfPaymentUrl { get; set; }
        
        // Navigation property - ForeignKey reference should be safe
        [ForeignKey("BookingId")]
        public virtual Booking? Booking { get; set; }
    }
    
    public enum PaymentMethod
    {
        Cash,
        BankTransfer,
        MobileMoney,
        CreditCard,
        Other
    }
    
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }
}