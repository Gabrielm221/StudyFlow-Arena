using Microsoft.EntityFrameworkCore;
using StudyFlowArena.API.Models;

namespace StudyFlowArena.API.Data{
        public class AppDbContext : DbContext{
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
            public DbSet<User> Users{ get; set; }
        }
}