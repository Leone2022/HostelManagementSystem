using HostelMS.Models;
using System.Collections.Generic;

namespace HostelMS.ViewModels
{
    public class RoomAssignmentViewModel
    {
        public Hostel Hostel { get; set; } = new Hostel();
        public Room Room { get; set; } = new Room();
        public List<ApplicationUser> CurrentOccupants { get; set; } = new List<ApplicationUser>();
        public List<ApplicationUser> AvailableStudents { get; set; } = new List<ApplicationUser>();
    }
}