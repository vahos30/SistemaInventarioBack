using System;

namespace SistemaInventario.Application.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos para un cliente.
    /// </summary>
    public class ClienteDto
    {
        public Guid Id { get; set; }
        public string NumeroDocumento { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CodigoCiudad { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public int CiudadId { get; set; }

        // Nuevos campos para Factus
        public int IdTipoOrganizacion { get; set; }
        public int IdTributo { get; set; }
        public int IdTipoDocumentoIdentidad { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
    }
}
