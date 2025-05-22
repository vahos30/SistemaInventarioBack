using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Application.DTOs
{
    public class CompraReporteDto
    {
        public Guid CompraId { get; set; }
        public DateTime Fecha { get; set; }
        public string ProveedorNombre { get; set; }
        public decimal Total { get; set; }

        public List<CompraDetalleReporteDto> Detalles { get; set; }
    }
}
