using Microsoft.EntityFrameworkCore;
using StudyFlowArena.API.Data;
using StudyFlowArena.API.Interfaces;
using StudyFlowArena.API.Models;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlowArena.API.Repositories{
    public class AuthRepository : IAuthRepository{

        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context){
            _context = context;
        }

        public async Task<User> Register(User user, string password){
            using var hmac = new HMACSHA512();
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> Login(string email, string password){
            var user = await _context.Users 
                .FirstOrDefaultAsync(x => x.Email == email);
            
            if(user == null)
                return null;

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            if(!ComputeHash.SequenceEqual(user.PasswordHash))
                return null;
            
            return user;
        }

        public async Task<bool> UserExists(string email){
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

    
    }
}