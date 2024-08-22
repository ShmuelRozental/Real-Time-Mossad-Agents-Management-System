using System.ComponentModel.DataAnnotations.Schema;

namespace Real_Time_Mossad_Agents_Management_System.Models
{
    public class Target
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }

       
        public PinLocation? Location { get; set; }

        public string PotoUrl { get; set; }

        public bool Status { get; set; }
    }
}
