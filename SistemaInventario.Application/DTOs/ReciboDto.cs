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
        public string NumeroDocumento { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }

        public List<DetalleReciboDto> Detalle { get; set; } = new();
    }
}


