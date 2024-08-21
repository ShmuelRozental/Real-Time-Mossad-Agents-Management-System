using System.ComponentModel.DataAnnotations.Schema;

namespace Real_Time_Mossad_Agents_Management_System.Models
{
    public class Agent
    {
        public int Id { get; set; }
        public string NikName { get; set; }

        [NotMapped]
        public PinLocation Location { get; set; }

        public bool Status { get; set; }
    }
}
