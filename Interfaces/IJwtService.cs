using Real_Time_Mossad_Agents_Management_System.Models;

namespace Real_Time_Mossad_Agents_Management_System.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user);
    }
}
