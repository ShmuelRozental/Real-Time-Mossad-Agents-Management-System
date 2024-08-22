using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Real_Time_Mossad_Agents_Management_System.Models
{
    [Owned]
    public class PinLocation
    {
        [Range(0, 1000, ErrorMessage = "X must be between 0 and 1000.")]
        public int X { get; set; }
        [Range(0, 1000, ErrorMessage = "Y must be between 0 and 1000.")]
        public int Y { get; set; }
    }
}
