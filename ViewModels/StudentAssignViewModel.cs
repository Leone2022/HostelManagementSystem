using HostelMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HostelMS.ViewModels
{
    public class StudentAssignViewModel
    {
        public ApplicationUser Student { get; set; } = new ApplicationUser();
        
        public List<Hostel> Hostels { get; set; } = new List<Hostel>();

        [Required(ErrorMessage = "Please select a hostel")]
        [Display(Name = "Hostel")]
        public int? SelectedHostelId { get; set; }

        [Required(ErrorMessage = "Please select a room")]
        [Display(Name = "Room Number")]
        public string? SelectedRoomNumber { get; set; }

        [Display(Name = "Temporary Assignment")]
        public bool IsTemporaryAssignment { get; set; } = false;

        [Display(Name = "Probation End Date")]
        [DataType(DataType.Date)]
        public DateTime? ProbationEndDate { get; set; }
    }
}