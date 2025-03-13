using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SistemaInventario.Application.DTOs;

namespace SistemaInventario.Application.Feactures.Reportes
{

    /// <summary>
    /// Query para obtener el inventario actual (Lista de productos con stock, precio ect.).
    /// 
    public record ObtenerInventarioQuery() : IRequest<IEnumerable<ProductoDto>>;
    
    
}
