namespace Real_Time_Mossad_Agents_Management_System.Models
{
    public class Agent
    {
        public int Id { get; set; }
        public string NikName { get; set; }
        public PinLocation PinLocation { get; set; }

        public bool Status { get; set; }
    }
}
