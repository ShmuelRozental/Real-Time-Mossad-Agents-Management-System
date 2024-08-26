using Microsoft.EntityFrameworkCore;
using Real_Time_Mossad_Agents_Management_System.Data;
using Real_Time_Mossad_Agents_Management_System.Enums;
using Real_Time_Mossad_Agents_Management_System.Interface.MissionsManagement.Interfaces;
using Real_Time_Mossad_Agents_Management_System.Interface;
using Real_Time_Mossad_Agents_Management_System.Models;

namespace Real_Time_Mossad_Agents_Management_System.services
{
    public class TargetsServices : IEntityService<Target>
    {
        private readonly AppDbContext _dbContext;
        private readonly IManagementServices<Target> _managementServices;

        public TargetsServices(AppDbContext dbContext, IManagementServices<Target> managementServices)
        {
            _dbContext = dbContext;
            _managementServices = managementServices;

        }


        public async Task<Target> GetEntityAsync(int targetId)
        {
            var target = await _dbContext.Targets.FindAsync(targetId);
            if (target == null)
            {
                throw new KeyNotFoundException("Target not found.");
            }
            return target;
        }

        public async Task<List<Target>> GetAllEntityAsync()
        {
            var targets = await _dbContext.Targets.ToListAsync();
            if (targets == null || targets.Count == 0)
            {
                throw new Exception("failed retrive Targets");
            }
            return targets;
        }

        public async Task<Target> CreateEntity(Target newTarget)
        {
            try
            {
                await _dbContext.Targets.AddAsync(newTarget);
                await _dbContext.SaveChangesAsync();
                return newTarget;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error saving target to the database.", ex);
            }
        }

        public async Task<Target> SetEntityLocation(int targetId, Location location)
        {
            var target = await _dbContext.Targets.FindAsync(targetId);
            if (target == null)
            {
                throw new Exception("This target does not exist.");
            }
            if (target.Location != null)
            {
                throw new Exception("This target already has a location set.");
            }

            target.Location = new Location
            {
                X = location.X,
                Y = location.Y
            };

            await _dbContext.SaveChangesAsync();
            await _managementServices.TryCreateMissionAsync(target);
            await _managementServices.TryDeleteMissionAsync(target);
            return target;
        }

        public async Task<Target> EntityMovement(int targetId, Direction direction)
        {
            var target = await _dbContext.Targets.FindAsync(targetId);
            if (target == null)
            {
                throw new KeyNotFoundException("Target not found.");
            }
            if (target.Location == null)
            {
                throw new InvalidOperationException("Target does not have a location set.");
            }

            target.Location.Move(direction);
            _dbContext.Targets.Update(target);
            await _dbContext.SaveChangesAsync();
            await _managementServices.TryCreateMissionAsync(target);

            return target;
        }
    }
}

}
