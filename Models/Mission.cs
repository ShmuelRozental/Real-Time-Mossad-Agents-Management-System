namespace Real_Time_Mossad_Agents_Management_System.Models
{
    public class Mission
    {
        public int Id { get; set; }

        public int AgentId { get; set; }
        public Agent Agent { get; set; }
        public DateTime TimeLeft { get; set; }



    }
}
