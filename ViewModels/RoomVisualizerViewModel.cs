using HostelMS.Models;
using System.Collections.Generic;

namespace HostelMS.ViewModels
{
    public class RoomVisualizerViewModel
    {
        public Hostel Hostel { get; set; } = new Hostel();
        public Dictionary<string, List<Room>> Floors { get; set; } = new Dictionary<string, List<Room>>();
    }
}