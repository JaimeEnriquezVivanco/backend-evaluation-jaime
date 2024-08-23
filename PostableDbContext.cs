using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Postable.Entities;

namespace Postable
{
    public class PostableDbContext : DbContext
    {
        public PostableDbContext(
            DbContextOptions<PostableDbContext> options
        )
            : base(options)
        {
        }

        protected override void
        OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfigurationsFromAssembly(
                Assembly.GetExecutingAssembly()
            );
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Like> Likes => Set<Like>();
    }
}