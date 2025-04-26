using StudyFlowArena.API.Models;
using System.Threading.Tasks;

namespace StudyFlowArena.API.Interfaces{
    public interface IAuthRepository {
        Task<User> Register(User user, string password);
        Task<User> Login(string email, string password);
        Task<bool> UserExists(string email);
        
    }
}