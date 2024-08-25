using Real_Time_Mossad_Agents_Management_System.Models;

namespace Real_Time_Mossad_Agents_Management_System.Interface
{
    public interface IAgentTargetService
    {
        void CheckAndUpdateAssignments(Agent agent);
        void CheckAndUpdateAssignments(Target target);
    }
}
