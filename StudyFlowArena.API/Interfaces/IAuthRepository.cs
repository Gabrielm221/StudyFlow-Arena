using StudyFlowArena.API.Models;

namespace StudyFlowArena.API.Interfaces{
    public interface IAuthRepository {
        Task<User> Register(User user, string password);
        Task<User> Login(string email, string password);
        task<bool> UserExists(string email);
        
    }
}