using Microsoft.EntityFrameworkCore;
using Real_Time_Mossad_Agents_Management_System.Data;
using Real_Time_Mossad_Agents_Management_System.Enums;
using Real_Time_Mossad_Agents_Management_System.Interfaces;
using Real_Time_Mossad_Agents_Management_System.Models;

namespace Real_Time_Mossad_Agents_Management_System.Services
{
    public class MissionsServices<T>
    {
        private readonly AppDbContext _dbContext;
        private readonly IManagementServices<T> _imanagementServices;

        public MissionsServices(AppDbContext dbContext, IManagementServices<T> managementServices)
        {
            _dbContext = dbContext;
            _imanagementServices = managementServices;
        }

        public async Task<Mission> GetAsync(int missionId)
        {
            var mission = await _dbContext.Missions.FindAsync(missionId);
            if (mission == null)
            {
                throw new KeyNotFoundException("Mission not found.");
            }
            return mission;
        }

        public async Task<List<Mission>> GetAllAsync()
        {
            var missions = await _dbContext.Missions.ToListAsync();
            if ( missions.Count == 0)
            {
                throw new Exception("failed retrive Targets");
            }
            return missions;
        }

      
        public async Task<Mission> UpdateStatus(int missionId, MissionStatus status)
        {
            var mission = await _dbContext.Missions
                .Include(m => m.Agent)
                .Include(m => m.Target)
                .FirstOrDefaultAsync(m => m.Id == missionId);

            if (mission == null)
            {
                throw new KeyNotFoundException("mission not found.");
            }
            if (mission.Agent == null)
            {
                throw new InvalidOperationException("Mission agent is null.");
            }

            if (mission.Target == null)
            {
                throw new InvalidOperationException("Mission target is null.");
            }

            if (mission.Agent.Location == null || mission.Agent.ActiveStatus)
            {
                throw new InvalidOperationException("Agent location is null  or on a miision.");
            }

            if (mission.Target.Location == null || mission.Target.Status)
            {
                throw new InvalidOperationException("Target location is null or the target already killed.");
            }
            if (_imanagementServices.CalculateDistance(mission.Agent.Location, mission.Target.Location) > 200)
            {
                throw new InvalidOperationException("target is to far distance from agent");
            }

            mission.Status = status;

            if (status == MissionStatus.Assigned)
            {
                mission.Agent.ActiveStatus = true;
            }
            else if (status == MissionStatus.Completed || status == MissionStatus.Offer)
            {
                mission.Agent.ActiveStatus = false;
            }
            _dbContext.Update(mission.Agent);
            await _dbContext.SaveChangesAsync();

            double remainingTime = _imanagementServices
                .CalculateDistance(mission.Agent.Location, mission.Target.Location) /5;

            mission.TimeLeft = TimeSpan.FromHours(remainingTime);

            var offeredMissionsToRemove = await _dbContext.Missions
                .Where(m =>
                    (m.AgentId == mission.Agent.Id && m.Status == MissionStatus.Offer) ||
                    (m.TargetId == mission.Target.Id && m.Status == MissionStatus.Offer))
                .ToListAsync();

            _dbContext.Missions.RemoveRange(offeredMissionsToRemove);

            _dbContext.Update(mission);
            await _dbContext.SaveChangesAsync();
            return mission;
        }

        public async Task StartMissionsAsync()
        {
            var missionsToStart = await _dbContext.Missions
                 .Include(m => m.Agent)
                 .Include(m => m.Target)
                 .Where(m => m.Status == MissionStatus.Assigned)
                 .ToListAsync();

            foreach (var mission in missionsToStart)
            {
                await _imanagementServices.StartMissionAsync(mission);
            }
        }
    }
}
