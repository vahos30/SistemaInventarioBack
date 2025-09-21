using MediatR;
using SistemaInventario.Application.DTOs;
using System;
using System.Collections.Generic;

namespace SistemaInventario.Application.Feactures.Facturas
{
    public class CrearFacturaFactusCommand : IRequest<string>
    {
        public Guid ClienteId { get; set; }
        public List<DetalleFacturaFactusDto> Detalles { get; set; }
        public string Referencia { get; set; }
        public string Observacion { get; set; }
        public string FechaVencimiento { get; set; }
        public string FormaPago { get; set; }
        public string MetodoPago { get; set; }
        
    }
}
