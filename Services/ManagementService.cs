using Microsoft.EntityFrameworkCore;
using Real_Time_Mossad_Agents_Management_System.Data;
using Real_Time_Mossad_Agents_Management_System.Enums;
using Real_Time_Mossad_Agents_Management_System.Interfaces;
using Real_Time_Mossad_Agents_Management_System.Models;
using System;
using System.Drawing;

namespace Real_Time_Mossad_Agents_Management_System.Services
{
    public class ManagementServices<T> : IManagementServices<T>
    {
        private readonly AppDbContext _dbContext;

        public ManagementServices(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool IsWithinDistance(Location agentLocation, Location targetLocation)
        {
            if (agentLocation.X == null || agentLocation.Y == null || targetLocation.X == null || targetLocation.Y == null)
            {
                throw new InvalidOperationException("Entities must have a location set to calculate distance.");
            }



            var xDistance = targetLocation.X - agentLocation.X;
            var yDistance = targetLocation.Y - agentLocation.Y;

            var distance = Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));

            return distance < 200;
        }

        public double CalculateDistance(Location loc1, Location loc2)
        {
            double x1 = loc1.X;
            double y1 = loc1.Y;
            double x2 = loc2.X;
            double y2 = loc2.Y;

            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
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

        public async Task TryCreateMissionAsync(T entity)
        {
            switch (entity)
            {
                case Agent agent:

                    var targets = await _dbContext.Targets.Where(t => !t.Status).ToListAsync();
                    foreach (var target in targets)
                    {

                        bool hasTargetWhitAsignMission = await HasActiveMissionAsync(target);
                        bool hasAgentTargetMission = await HasActiveMissionForAgentAndTargetAsync(agent.Id, target.Id);
                        if (!agent.ActiveStatus &&
                            CalculateDistance(
                                agent.Location, target.Location
                            ) < 200
                            && !hasTargetWhitAsignMission && !hasAgentTargetMission)
                        {
                            await CreateAsync(agent, target);
                        }
                    }
                    break;

                case Target target:
                    bool targetWhitAsignMission = await HasActiveMissionAsync(target);

                    if (targetWhitAsignMission)
                    {
                        break;
                    }
                    var agents = await _dbContext.Agents.Where(a => !a.ActiveStatus).ToListAsync();
                    foreach (var agent in agents)
                    {
                        bool hasAgentTargetMission = await HasActiveMissionForAgentAndTargetAsync(agent.Id, target.Id);
                        if (IsWithinDistance(target.Location, agent.Location) && !hasAgentTargetMission)
                        {

                            await CreateAsync(agent, target);
                        }
                    }
                    break;

                default:
                    throw new InvalidOperationException("Unsupported entity type for mission creation.");
            }
        }

        public async Task TryDeleteMissionAsync(T entity)
        {
            switch (entity)
            {
                case Target target:
                    var targetMisions = await _dbContext.Missions.Where(m => m.TargetId == target.Id).ToListAsync();
                    foreach (var mision in targetMisions)
                    {

                        if (mision.Status == MissionStatus.Offer && CalculateDistance(mision.Agent.Location, target.Location) > 200)
                        {
                            await DeleteMissionAsync(mision.Id);
                        }
                    }
                    break;

                case Agent agent:
                    var agentMisions = await _dbContext.Missions.Where(m => m.AgentId == agent.Id).ToListAsync();
                    foreach (var mision in agentMisions)
                    {


                        if (mision.Status == MissionStatus.Offer && CalculateDistance(mision.Target.Location, agent.Location) > 200)
                        {
                            await DeleteMissionAsync(mision.Id);
                        }
                    }
                    break;

                default:
                    throw new InvalidOperationException("Unsupported entity type for mission creation.");
            }
        }

        public async Task StartMissionAsync(Mission mission)
       {
            if (mission.Agent == null || mission.Target == null)
            {
                throw new ArgumentNullException("Entities cannot be null");
            }

            var agentLocation = mission.Agent.Location;
            var targetLocation = mission.Target.Location;

            if (agentLocation == null || targetLocation == null)
            {
                throw new InvalidOperationException("Both entities must have a location set.");
            }

            Direction nextMove = DetermineDirection(agentLocation, targetLocation);
            mission.Agent.Location.Move(nextMove);

            if (HasReachedTarget(mission.Agent.Location, mission.Target.Location))
            {
                await EndMissionAsync(mission);
            }
            _dbContext.SaveChanges();
        }

        public async Task EndMissionAsync(Mission mission)
        {
            mission.Status = MissionStatus.Completed;
            mission.Target.Status = true;
            mission.Agent.ActiveStatus = false; 

            _dbContext.Update(mission);
            await _dbContext.SaveChangesAsync();

        }

        public static Direction DetermineDirection(Location agent, Location target)
        {
            double deltaX = target.X - agent.X;
            double deltaY = target.Y - agent.Y;

            if (deltaX == 0)
            {
                return deltaY > 0 ? Direction.E : Direction.W;
            }
            else if (deltaY == 0)
            {
                return deltaX > 0 ? Direction.S : Direction.N;
            }
            else
            {
                if (deltaX > deltaY)
                {
                    return deltaX > 0
                        ? (deltaY > 0 ? Direction.SE : Direction.SW)
                        : (deltaY > 0 ? Direction.NE : Direction.NW);
                }
                else
                {
                    return deltaY > 0
                        ? (deltaX > 0 ? Direction.SE : Direction.NE)
                        : (deltaX > 0 ? Direction.SW : Direction.NW);
                }
            }
        }


        private async Task<bool> HasActiveMissionForAgentAndTargetAsync(int agentId, int targetId)
        {
            return await _dbContext.Missions
                .AnyAsync(m => m.AgentId == agentId && m.TargetId == targetId);
        }

        public bool HasReachedTarget(Location agent, Location target, double tolerance = 0.01)
        {
            double distance = CalculateDistance(agent, target);
            return distance <= tolerance;
        }

        public async Task<bool> HasActiveMissionAsync(Target target)
        {
            return await _dbContext.Missions
                .AnyAsync(m => m.TargetId == target.Id && m.Status != MissionStatus.Offer);
        }
    }
}

