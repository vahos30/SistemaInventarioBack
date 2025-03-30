using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Application.DTOs
{
    /// <summary>
    /// Objeo de transferencia de datos para un producto.
    ///     
    public class ProductoDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int CantidadStock { get; set; }

        public bool Activo { get; set; }
    }
}
