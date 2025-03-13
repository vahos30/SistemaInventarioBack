using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Application.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos para un detalle de recibo.
    public class DetalleReciboDto
    {
      
        public Guid ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
