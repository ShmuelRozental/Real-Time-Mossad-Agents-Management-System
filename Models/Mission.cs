
using Real_Time_Mossad_Agents_Management_System.Enums;

namespace Real_Time_Mossad_Agents_Management_System.Models
{
    public class Mission
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int TargetId { get; set; }
        public MissionStatus Status { get; set; }
        public TimeSpan TimeLeft { get; set; }
        public Agent Agent { get; set; }
        public Target Target { get; set; }

    }
}
