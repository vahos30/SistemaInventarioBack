using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SistemaInventario.Infrastructure.Persistence
{
    /// <summary>
    /// Fábrica en tiempo de diseño para crear una instancia de AppDbContext.
    /// Permite a EF Core crear el DbContext cuando se ejecutan migraciones.
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Construir la configuración desde el appsettings.json ubicado en la raíz del proyecto API.
            // Nota: El directorio actual cuando se ejecuta este comando es el de SistemaInventario.Infrastructure,
            // por lo que es posible que necesitemos ajustar la ruta.
            var basePath = Directory.GetCurrentDirectory();

            // Opcionalmente, puedes cambiar el directorio base si el appsettings.json está en el proyecto API:
            // var basePath = Path.Combine(Directory.GetCurrentDirectory(), @"..\SistemaInventario.API");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            // Obtener la cadena de conexión
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                // Si no se encontró en appsettings.json, puedes especificarla directamente:
                connectionString = "Server=JONATHAN-VAHOS\\SQLEXPRESS;Database=InventarioDB;Trusted_Connection=True;MultipleActiveResultSets=true";
            }

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}

