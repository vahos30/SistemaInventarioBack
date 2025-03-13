using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Domain.Entities;

namespace SistemaInventario.Infrastructure.Persistence
{
    // Dbcontext para el sistema de inventario
    // Aqui se configura las entidades y las relaiones con SQL Server
    public class AppDbContext : DbContext
    {
        // se inyectan las opciones del DbContext a traves del constructor
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Definicion de los DbSet para las entidades
        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<Recibo> Recibos { get; set; } = null!;
        public DbSet<DetalleRecibo> DetallesRecibo { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuramos las claves Primarias
            modelBuilder.Entity<Cliente>().HasKey(c => c.Id);
            modelBuilder.Entity<Producto>().HasKey(p => p.Id);
            modelBuilder.Entity<Recibo>().HasKey(r => r.Id);
            modelBuilder.Entity<DetalleRecibo>().HasKey(d => d.Id);

            // Configuramos las relacion entre Factura y Cliente
            modelBuilder.Entity<Recibo>()
                .HasOne(r => r.Cliente)
                .WithMany()
                .HasForeignKey(r => r.ClienteId);

            // Configuramos la relacion entre DetalleRecibo y Recibo
            modelBuilder.Entity<DetalleRecibo>()
                .HasOne(d => d.Recibo)
                .WithMany(r => r.Detalles)
                .HasForeignKey(d => d.ReciboId);

            // Configuramos la relacion entre DetalleRecibo y Producto
            modelBuilder.Entity<DetalleRecibo>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ProductoId);
        }
    }
}
