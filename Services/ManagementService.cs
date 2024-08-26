using Real_Time_Mossad_Agents_Management_System.Data;
using Real_Time_Mossad_Agents_Management_System.Enums;
using Real_Time_Mossad_Agents_Management_System.Interface.MissionsManagement.Interfaces;
using Real_Time_Mossad_Agents_Management_System.Models;
using System;
using System.Drawing;

namespace Real_Time_Mossad_Agents_Management_System.services
{
    public class ManagementServices<T> : IManagementServices<T>
        {
            private readonly AppDbContext _dbContext;
            private readonly MissionsServices<T> _missionsServices;

            public ManagementServices(AppDbContext dbContext, MissionsServices<T> missionServices)
            {
                _dbContext = dbContext;
                _missionsServices = missionServices;
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

            public async Task TryCreateMissionAsync(T entity)
            {
                switch (entity)
                {
                    case Agent agent:

                        var targets = await _dbContext.Targets.Where(t => !t.Status).ToListAsync();
                        foreach (var target in targets)
                        {

                            bool hasTargetWhitAsignMission = await HasActiveMissionAsync(target);
                            if (!agent.ActiveStatus &&
                                IsWithinDistance(
                                    agent.Location, target.Location
                                )
                                && !hasTargetWhitAsignMission)
                            {
                                await _missionsServices.CreateAsync(agent, target);
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
                            if (IsWithinDistance(target.Location, agent.Location))
                            {

                                await _missionsServices.CreateAsync(agent, target);
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

                            if (mision.Status == MissionStatus.Offer && !IsWithinDistance(mision.Agent.Location, target.Location))
                            {
                                await _missionsServices.DeleteMissionAsync(mision.Id);
                            }
                        }
                        break;

                    case Agent agent:
                        var agentMisions = await _dbContext.Missions.Where(m => m.AgentId == agent.Id).ToListAsync();
                        foreach (var mision in agentMisions)
                        {


                            if (mision.Status == MissionStatus.Offer && !IsWithinDistance(mision.Target.Location, agent.Location))
                            {
                                await _missionsServices.DeleteMissionAsync(mision.Id);
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

                if (mission.Agent.Location == mission.Target.Location)
                {
                    await EndMissionAsync(mission);
                }
                _dbContext.SaveChanges();
            }

            public async Task EndMissionAsync(Mission mission)
            {
                mission.Status = MissionStatus.Finished;
                mission.Target.Status = true;

                _dbContext.Update(mission);
                await _dbContext.SaveChangesAsync();

            }

            public static Direction DetermineDirection(Location agent, Location target)
            {
                double deltaX = target.X - agent.X;
                double deltaY = target.Y - agent.Y;

                if (deltaX == 0)
                {
                    return deltaY > 0 ? Direction.N : Direction.S;
                }
                else if (deltaY == 0)
                {
                    return deltaX > 0 ? Direction.E : Direction.W;
                }
                else
                {
                    if (Math.Abs(deltaX) > Math.Abs(deltaY))
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

            public async Task<bool> HasActiveMissionAsync(Target target)
            {
                return await _dbContext.Missions
                    .AnyAsync(m => m.TargetId == target.Id && m.Status != MissionStatus.Offer);
            }
        }
    }

}
