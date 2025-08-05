using Microsoft.EntityFrameworkCore;
using SistemaInventario.Domain.Entities;

namespace SistemaInventario.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Recibo> Recibos { get; set; }
        public DbSet<DetalleRecibo> DetallesRecibo { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Compra> Compras { get; set; }                 // <- Agregado
        public DbSet<DetalleCompra> DetallesCompra { get; set; }   // <- Agregado
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<DetalleFactura> DetallesFactura { get; set; }
        public DbSet<NotaCredito> NotasCredito { get; set; }
        public DbSet<DetalleNotaCredito> DetallesNotaCredito { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de claves
            modelBuilder.Entity<Cliente>().HasKey(c => c.Id);
            modelBuilder.Entity<Producto>().HasKey(p => p.Id);
            modelBuilder.Entity<Recibo>().HasKey(r => r.Id);
            modelBuilder.Entity<DetalleRecibo>().HasKey(d => d.Id);
            modelBuilder.Entity<Proveedor>().HasKey(p => p.Id);
            modelBuilder.Entity<Compra>().HasKey(c => c.Id);                // <- Agregado
            modelBuilder.Entity<DetalleCompra>().HasKey(d => d.Id);         // <- Agregado
            modelBuilder.Entity<Factura>().HasKey(f => f.Id);
            modelBuilder.Entity<DetalleFactura>().HasKey(d => d.Id);
            modelBuilder.Entity<NotaCredito>().HasKey(nc => nc.Id);
            modelBuilder.Entity<DetalleNotaCredito>().HasKey(d => d.Id);

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasIndex(c => c.NumeroDocumento)
                    .IsUnique();
            });

            // Índice único para NIT de proveedor
            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.HasIndex(p => p.NIT)
                    .IsUnique();
            });

            // Relación Compra -> Proveedor
            modelBuilder.Entity<Compra>()
                .HasOne(c => c.Proveedor)
                .WithMany() // Si quieres la navegación inversa, cambia a .WithMany(p => p.Compras)
                .HasForeignKey(c => c.ProveedorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Compra -> DetallesCompra
            modelBuilder.Entity<Compra>()
                .HasMany(c => c.Detalles)
                .WithOne(d => d.Compra)
                .HasForeignKey(d => d.CompraId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación DetalleCompra -> Producto
            modelBuilder.Entity<DetalleCompra>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Recibo -> Cliente
            modelBuilder.Entity<Recibo>()
                .HasOne(r => r.Cliente)
                .WithMany(c => c.Recibos)
                .HasForeignKey(r => r.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación DetalleRecibo -> Recibo (Configuración principal)
            modelBuilder.Entity<DetalleRecibo>()
                .HasOne(d => d.Recibo)
                .WithMany(r => r.Detalles)
                .HasForeignKey(d => d.ReciboId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación DetalleRecibo -> Producto
            modelBuilder.Entity<DetalleRecibo>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Factura -> Cliente
            modelBuilder.Entity<Factura>()
                .HasOne(f => f.Cliente)
                .WithMany()
                .HasForeignKey(f => f.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Factura -> DetallesFactura
            modelBuilder.Entity<Factura>()
                .HasMany(f => f.Detalles)
                .WithOne(d => d.Factura)
                .HasForeignKey(d => d.FacturaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación DetalleFactura -> Producto
            modelBuilder.Entity<DetalleFactura>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación NotaCredito -> Factura
            modelBuilder.Entity<NotaCredito>()
                .HasOne(nc => nc.Factura)
                .WithOne(f => f.NotaCredito)
                .HasForeignKey<NotaCredito>(nc => nc.FacturaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación NotaCredito -> DetallesNotaCredito
            modelBuilder.Entity<NotaCredito>()
                .HasMany(nc => nc.Detalles)
                .WithOne(d => d.NotaCredito)
                .HasForeignKey(d => d.NotaCreditoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
