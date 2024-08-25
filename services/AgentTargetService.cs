using Real_Time_Mossad_Agents_Management_System.Data;
using Real_Time_Mossad_Agents_Management_System.Enums;
using Real_Time_Mossad_Agents_Management_System.Interface;
using Real_Time_Mossad_Agents_Management_System.Models;
using System.Linq;

namespace Real_Time_Mossad_Agents_Management_System.services
{
    public class AgentTargetService : IAgentTargetService
    {
        private readonly ApplicationDbContext _context;

        public AgentTargetService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void CheckAndUpdateAssignments(Agent agent)
        {
           
            var targets = _context.Targets.ToList();
            foreach (var target in targets)
            {
                double distance = CalculateDistance(agent.Location, target.Location);
                if (distance <= 200)
                {
                    var mission = CreateMission(agent, target);
                    if (!IsMissionAssigned(mission))
                    {
                        AssignMission(mission, mission.Status);
                    }
                }
                else
                {
                    RemoveMissionIfExists(agent, target);
                }
            }
        }

        public void CheckAndUpdateAssignments(Target target)
        {
            // Use the existing context
            var agents = _context.Agents.ToList();
            foreach (var agent in agents)
            {
                double distance = CalculateDistance(agent.Location, target.Location);
                if (distance <= 200)
                {
                    var mission = CreateMission(agent, target);
                    if (!IsMissionAssigned(mission))
                    {
                        AssignMission(mission, MissionStatus.Offer);
                    }
                }
                else
                {
                    RemoveMissionIfExists(agent, target);
                }
            }
        }

        private double CalculateDistance(PinLocation loc1, PinLocation loc2)
        {
            return Math.Sqrt(Math.Pow(loc2.X - loc1.X, 2) + Math.Pow(loc2.Y - loc1.Y, 2));
        }

        private Mission CreateMission(Agent agent, Target target)
        {
            return new Mission
            {
                AgentId = agent.Id,
                TargetId = target.Id,
                Status = MissionStatus.Offer
            };
        }

        private bool IsMissionAssigned(Mission mission)
        {
            return _context.Missions.Any(m => m.AgentId == mission.AgentId && m.TargetId == mission.TargetId && m.Status != MissionStatus.Finished);
        }

        private void AssignMission(Mission mission, MissionStatus status)
        {
            mission.Status = status;
            _context.Missions.Add(mission);
            _context.SaveChanges();
        }

        private void RemoveMissionIfExists(Agent agent, Target target)
        {
            var mission = _context.Missions.FirstOrDefault(m => m.AgentId == agent.Id && m.TargetId == target.Id);
            if (mission != null && mission.Status != MissionStatus.Finished)
            {
                _context.Missions.Remove(mission);
                _context.SaveChanges();
            }
        }
    }
}
