// Path: Models/Room.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostelMS.Models
{
    public class Room
    {
        public Room()
        {
            OccupyingStudents = new HashSet<ApplicationUser>();
            MaintenanceRequests = new HashSet<MaintenanceRequest>();
        }

        [Key]
        public int RoomId { get; set; }
        
        [Required]
        public string RoomNumber { get; set; } = string.Empty;
        
        [Required]
        public int HostelId { get; set; }
        
        [Required]
        public RoomType Type { get; set; }
        
        [Required]
        public int Capacity { get; set; }
        
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerSemester { get; set; }
        
        // Updated statuses to reflect assignment rather than booking
        public RoomStatus Status { get; set; } = RoomStatus.Available;
        
        public int CurrentOccupancy { get; set; } = 0;
        
        public bool NeedsAttention { get; set; } = false;
        
        // Navigation properties
        [ForeignKey("HostelId")]
        public virtual Hostel? Hostel { get; set; }
        
        public virtual ICollection<ApplicationUser> OccupyingStudents { get; set; }
        public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }
    }
    
    public enum RoomType
    {
        Single,
        Double,
        Triple,
        Dormitory
    }
    
    public enum RoomStatus
    {
        Available,
        PartiallyAssigned,
        FullyAssigned,
        UnderMaintenance,
        Reserved
    }
}