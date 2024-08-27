using System.ComponentModel.DataAnnotations.Schema;
using Real_Time_Mossad_Agents_Management_System.Models;

namespace Real_Time_Mossad_Agents_Management_System.Models
{
    public class Target
    {

        public int Id { get; set; }
        public string name { get; set; }

        public string photoUrl { get; set; }
        public Location? Location { get; set; }

        public bool Status { get; set; } = false;
    }
}
