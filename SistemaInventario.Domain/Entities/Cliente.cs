using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Domain.Entities
{
    // Representa un cliente en el sistema
    public class Cliente
    {
        // Identificador unico (GUID)
        public Guid Id { get; set; } = Guid.NewGuid();

        // Nombre del cliente
        public string Nombre { get; set; } = string.Empty;

        // Apellido del cliente
        public string Apellido { get; set; } = string.Empty;

        // Tipo de documento
        public string TipoDocumento { get; set; } = string.Empty;

        // Numero de documento del cliente
        public string NumeroDocumento { get; set; } = string.Empty;

        // Telefono del cliente
        public string Telefono { get; set; } = string.Empty;

        // Direccion del cliente
        public string Direccion { get; set; } = string.Empty;

        // Email del cliente
        public string Email { get; set; } = string.Empty;

        // Navegación inversa
        public List<Recibo> Recibos { get; set; } = new(); // ✅ Nueva propiedad

        // Código de la ciudad
        public string CodigoCiudad { get; set; } = string.Empty; // Ej: "05030"

        // Nombre de la ciudad
        public string Ciudad { get; set; } = string.Empty;       // Ej: "Amaga"

        // Nombre del departamento
        public string Departamento { get; set; } = string.Empty; // Ej: "Antioquia"

        // Nuevo campo para el id de la ciudad de Factus
        public int CiudadId { get; set; }

        // Nuevos campos para Factus
        public int IdTipoOrganizacion { get; set; }         // legal_organization_id
        public int IdTributo { get; set; }                  // tribute_id
        public int IdTipoDocumentoIdentidad { get; set; }   // identification_document_id

        // razón social del cliente
        public string RazonSocial { get; set; } = string.Empty;
    }
}
