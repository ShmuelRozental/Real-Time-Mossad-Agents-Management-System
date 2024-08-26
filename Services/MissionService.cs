using Microsoft.EntityFrameworkCore;
using Real_Time_Mossad_Agents_Management_System.Data;
using Real_Time_Mossad_Agents_Management_System.Enums;
using Real_Time_Mossad_Agents_Management_System.Interface.MissionsManagement.Interfaces;
using Real_Time_Mossad_Agents_Management_System.Models;

namespace Real_Time_Mossad_Agents_Management_System.services
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
                if (missions == null || missions.Count == 0)
                {
                    throw new Exception("failed retrive Targets");
                }
                return missions;
            }

            public async Task<Mission> CreateAsync(Agent agent, Target target)
            {
                var existingMission = await _dbContext.Missions
                    .FirstOrDefaultAsync(m => m.AgentId == agent.Id && m.TargetId == target.Id);

                if (existingMission != null)
                {
                    return existingMission;
                }
                try
                {
                    Mission newMission = new Mission()
                    {
                        AgentId = agent.Id,
                        TargetId = target.Id,
                        Status = MissionStatus.Offer
                    };
                    await _dbContext.Missions.AddAsync(newMission);
                    await _dbContext.SaveChangesAsync();
                    return newMission;
                }
                catch (DbUpdateException ex)
                {
                    throw new InvalidOperationException("Error saving Mission to the database.", ex);
                }
            }

            public async Task<Mission> UpdateStatus(int missionId, MissionStatus status)
            {
                var mission = await _dbContext.Missions.FindAsync(missionId);

                if (mission == null)
                {
                    throw new KeyNotFoundException("mission not found.");
                }
                if (!_imanagementServices.IsWithinDistance(mission.Agent.Location, mission.Target.Location))
                {
                    throw new InvalidOperationException("target is to far distance from agent");
                }

                mission.Status = status;
                mission.Agent.ActiveStatus = true;
                double remainingTime = _imanagementServices.CalculateDistance(mission.Agent.Location, mission.Target.Location);

                var offeredMissionsToRemove = await _dbContext.Missions
                  .Where(m =>
                      (m.AgentId == mission.Agent.Id && m.Status == MissionStatus.Offer) ||
                      (m.TargetId == mission.Target.Id && m.Status == MissionStatus.Offer))
                  .ToListAsync();

                _dbContext.Missions.RemoveRange(offeredMissionsToRemove);

                _dbContext.Update(mission);
                await _dbContext.SaveChangesAsync();
                return mission;

                _dbContext.Update(mission);
                await _dbContext.SaveChangesAsync();
            }

            public async Task StartMissionsAsync()
            {
                var missionToStart = await _dbContext.Missions.Where(m => m.Status == MissionStatus.assigned).ToListAsync();
                foreach (var mission in missionToStart)
                {
                    await _imanagementServices.StartMissionAsync(mission);
                }
            }

            public async Task DeleteMissionAsync(int missionId)
            {
                var mission = await _dbContext.Missions.FindAsync(missionId);
                if (mission == null)
                {
                    throw new KeyNotFoundException("mission not found.");
                }
                _dbContext.Remove(mission);
                await _dbContext.SaveChangesAsync();
            }
        }
    }

}
