using Microsoft.EntityFrameworkCore;
using Real_Time_Mossad_Agents_Management_System.Data;
using Real_Time_Mossad_Agents_Management_System.Enums;
using Real_Time_Mossad_Agents_Management_System.Interfaces;
using Real_Time_Mossad_Agents_Management_System.Models;

namespace Real_Time_Mossad_Agents_Management_System.Services
{
    public class AgentsServices : IEntityService<Agent>
    {

        private readonly AppDbContext _dbContext;
        private readonly IManagementServices<Agent> _managementServices;

        public AgentsServices(AppDbContext context, IManagementServices<Agent> managementServices)
        {
            _dbContext = context;
            _managementServices = managementServices;
        }


        public async Task<Agent> GetEntityAsync(int agentId)
        {
            var agent = await _dbContext.Agents.FindAsync(agentId);
            if (agent == null)
            {
                throw new KeyNotFoundException("Agent not found.");
            }
            return agent;
        }

        public async Task<List<Agent>> GetAllEntityAsync()
        {

            var agents = await _dbContext.Agents.ToListAsync();
            if (agents == null || agents.Count == 0)
            {
                throw new Exception("failed retrive Agents");
            }
            return agents;
        }

        public async Task<Agent> CreateEntity(Agent newAgent)
        {
            try
            {
                await _dbContext.Agents.AddAsync(newAgent);
                await _dbContext.SaveChangesAsync();
                return newAgent;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error saving agent to the database.", ex);
            }
        }

        public async Task<Agent> SetEntityLocation(int agentId, Location location)
        {
            var agent = await _dbContext.Agents.FindAsync(agentId);
            if (agent == null)
            {
                throw new KeyNotFoundException("Agent not found.");
            }
            if (agent.Location != null)
            {
                throw new InvalidOperationException("Agent already has a location set.");
            }

            agent.Location = new Location
            {
                X = location.X,
                Y = location.Y
            };

            await _dbContext.SaveChangesAsync();
            await _managementServices.TryCreateMissionAsync(agent);
            return agent;
        }

        public async Task<Agent> EntityMovement(int agentId, Direction direction)
        {
            var agent = await _dbContext.Agents.FindAsync(agentId);
            if (agent == null)
            {
                throw new KeyNotFoundException("Agent not found.");
            }
            if (agent.ActiveStatus)
            {
                throw new InvalidOperationException("Agent is currently on a mission.");
            }
            if (agent.Location == null)
            {
                throw new InvalidOperationException("Agent does not have a location set.");
            }
            agent.Location.Move(direction);

            _dbContext.Update(agent);
            await _dbContext.SaveChangesAsync();
            await _managementServices.TryCreateMissionAsync(agent);
            return agent;
        }
    }
}