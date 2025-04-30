using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostelMS.Models
{
    public class Amenity
    {
        [Key]
        public int AmenityId { get; set; }
        
        [Required]
        public int HostelId { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public string? IconClass { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation property
        [ForeignKey("HostelId")]
        public virtual Hostel? Hostel { get; set; }
    }
}