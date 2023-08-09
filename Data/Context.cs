using BlogAPI.Data.Mappings;
using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Data
{
    public class Context : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }

        // public DbSet<PostTag> PostTags { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }

        // public DbSet<UserRole> UserRoles { get; set; }

        //Realiza a conexão com BD
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=localhost,1433;Database=BlogAPI;User ID=sa;Password=1q2w3e4r@#$;TrustServerCertificate=True;");
            // options.LogTo();
        }

        //Criação dos modelos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryMap());
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new PostMap());
        }
    }
}