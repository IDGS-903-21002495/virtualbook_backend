using Microsoft.EntityFrameworkCore;
using virtualbook_backend.Models;

namespace virtualbook_backend.Data
{
    public class VirtualBookDbContext : DbContext
    {
        public VirtualBookDbContext()
        {
        }
        public VirtualBookDbContext(DbContextOptions<VirtualBookDbContext> options) : base(options)
        {

        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Libro> Libros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Libros)
                .WithOne(l => l.Usuario)
                .HasForeignKey(l => l.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
