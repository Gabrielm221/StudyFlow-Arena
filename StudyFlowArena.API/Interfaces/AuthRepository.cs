using Microsoft.EntityFrameworkCore;
using StudyFLowArena.API.Data;
using StudyFLowArena.API.Interfaces;
using StudyFLowArena.API.Models;
using System.Security.Cryptography;
using System.Text;

namespace StudyFLowArena.API.Repositories{
    public class AuthRepository : IAuthRepository{

        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context){
            _context = context
        }

        public async Task<User> Register(User user, string password){
            using var hmac = new HMACSHA512();
            user.setPasswordSalt(hmac.Key);
            user.setPasswordHash(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> Login(string email, string password){
            var user = await _context.Users 
                .FirstOrDefaultAsync(x => x.getEmail() == email)
            
            if(user == null)
                return null;

            using var hmac = new HMACSHA512(user.getPasswordSalt());
            var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            if(!ComputeHash.SequenceEqual(user.getPasswordHash()))
                return null;
            
            return user;
        }

        public async Task<bool> UserExists(strings email){
            return await _context.Users.AnyAsync(x => x.getEmail == email);
        }

    }
}