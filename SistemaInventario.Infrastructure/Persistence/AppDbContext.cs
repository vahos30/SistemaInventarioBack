using Microsoft.EntityFrameworkCore;
using SistemaInventario.Domain.Entities;

namespace SistemaInventario.Infrastructure.Persistence
{   public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Recibo> Recibos { get; set; }
        public DbSet<DetalleRecibo> DetallesRecibo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de claves
            modelBuilder.Entity<Cliente>().HasKey(c => c.Id);
            modelBuilder.Entity<Producto>().HasKey(p => p.Id);
            modelBuilder.Entity<Recibo>().HasKey(r => r.Id);
            modelBuilder.Entity<DetalleRecibo>().HasKey(d => d.Id);

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasIndex(c => c.NumeroDocumento)
                .IsUnique();
            });

            // Relación Recibo -> Cliente
            modelBuilder.Entity<Recibo>()
                .HasOne(r => r.Cliente)
                .WithMany(c => c.Recibos) // ✅ Navegación inversa
                .HasForeignKey(r => r.ClienteId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminar clientes con recibos

            // Relación DetalleRecibo -> Recibo (Configuración principal)
            modelBuilder.Entity<DetalleRecibo>()
                .HasOne(d => d.Recibo)
                .WithMany(r => r.Detalles)
                .HasForeignKey(d => d.ReciboId)
                .OnDelete(DeleteBehavior.Cascade); // Elimina detalles al borrar recibo

            // Relación DetalleRecibo -> Producto
            modelBuilder.Entity<DetalleRecibo>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminar productos con detalles
        }
    }
}
