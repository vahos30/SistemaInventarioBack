using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Application.DTOs
{

    /// <summary>
    /// Objeto de transferencia de datos para un recibo.
    /// 
    public class ReciboDto
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; } 
        public List<DetalleReciboDto> Detalles { get; set; } = new();
        public string FormaPago { get; set; } = string.Empty;
    }
}


