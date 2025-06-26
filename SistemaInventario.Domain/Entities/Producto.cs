using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Domain.Entities
{
    // Representa un producto en el inventario
    public class Producto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        //nombre del producto
        public string Nombre { get; set; } = string.Empty;

        //Precio del producto
        public decimal Precio { get; set; } = 0;

        //Referencia del producto
        public string Referencia { get; set; } = string.Empty;

        // Descripción del producto
        public string Descripcion { get; set; } = string.Empty;

        //cantidad de productos
        public int CantidadStock { get; set; }

        public bool Activo { get; set; }
    }
}
