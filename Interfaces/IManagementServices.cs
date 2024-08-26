using Real_Time_Mossad_Agents_Management_System.Enums;
using Real_Time_Mossad_Agents_Management_System.Models;

namespace Real_Time_Mossad_Agents_Management_System.Interfaces
{
        public interface IManagementServices<T>
        {
            bool IsWithinDistance(Location agentLocation, Location targetLocation);
            double CalculateDistance(Location loc1, Location loc2);
            Task TryCreateMissionAsync(T entity1);
            Task TryDeleteMissionAsync(T entity1);
            Task StartMissionAsync(Mission mission);
        }
   
}
