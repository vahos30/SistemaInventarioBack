using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Entities;

namespace SistemaInventario.Application.Mapping

/// <summary>
/// Configuración de mapeo de objetos.
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            // Mapear CLiente a clienteDto y viceversa
            CreateMap<Cliente, ClienteDto>().ReverseMap();

            // Mapear Producto a ProductoDto y viceversa
            CreateMap<Producto, ProductoDto>().ReverseMap();

            // Mapear Factura y Factura Detalle segun sea necesario
            CreateMap<Recibo, ReciboDto>().ReverseMap();
            CreateMap<DetalleRecibo, DetalleReciboDto>().ReverseMap();
        }
    }
}
