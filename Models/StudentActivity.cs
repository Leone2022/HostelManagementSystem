// Path: Models/StudentActivity.cs
using System.ComponentModel.DataAnnotations;

namespace HostelMS.Models
{
    public class StudentActivity
    {
        [Key]
        public int ActivityId { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string UserName { get; set; } = string.Empty;
        
        [Required]
        public string ActivityType { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}