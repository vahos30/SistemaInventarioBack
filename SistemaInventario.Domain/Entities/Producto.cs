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

        //descripcion del producto
        public decimal Precio { get; set; } = 0;

        //cantidad de productos
        public int Cantidad { get; set; } = 0;
    }
}
