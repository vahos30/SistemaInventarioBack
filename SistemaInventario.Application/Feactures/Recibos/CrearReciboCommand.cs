using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SistemaInventario.Application.DTOs;

namespace SistemaInventario.Application.Feactures.Recibos
{
    /// <summary>
    /// Comando para crear un nuevo recibo.
    /// </summary>
    public class CrearReciboCommand : IRequest<ReciboDto>
    {
        public Guid ClienteId { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public List<DetalleReciboDto> Detalles { get; set; } = new();

        public decimal ValorIva { get; set; }
    }
}
