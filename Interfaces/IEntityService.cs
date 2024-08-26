using Real_Time_Mossad_Agents_Management_System.Enums;
using Real_Time_Mossad_Agents_Management_System.Models;

namespace Real_Time_Mossad_Agents_Management_System.Interfaces
{
    public interface IEntityService<T>
    {

        Task<T> GetEntityAsync(int id);
        Task<List<T>> GetAllEntityAsync();
        Task<T> CreateEntity(T entity);
        Task<T> SetEntityLocation(int id, Location location);
        Task<T> EntityMovement(int agentId, Direction direction);
    }
}
