using System.ComponentModel.DataAnnotations;

namespace HostelMS.ViewModels
{
    public class AmenityViewModel
    {
        public int AmenityId { get; set; }
        
        public int HostelId { get; set; }
        
        public string HostelName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Amenity Name")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Description")]
        public string? Description { get; set; }
        
        [Display(Name = "Icon Class")]
        public string? IconClass { get; set; }
        
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}