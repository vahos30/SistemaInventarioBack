using System.Collections.Generic;
using System.Linq;
using SistemaInventario.Domain.Entities;

namespace SistemaInventario.Application.Mappers
{
    public static class FactusMapper
    {
        public static FactusCustomer MapClienteToFactusCustomer(Cliente cliente)
        {
            string nit = cliente.NumeroDocumento;
            string dv = "";

            if (cliente.TipoDocumento == "NIT")
            {
                if (nit.Contains("-"))
                {
                    var partes = nit.Split('-');
                    nit = partes[0];
                    dv = partes.Length > 1 ? partes[1] : CalcularDigitoVerificacion(nit);
                }
                else
                {
                    dv = CalcularDigitoVerificacion(nit);
                }
            }

            return new FactusCustomer
            {
                identification = nit,
                dv = dv,
                company = cliente.IdTipoOrganizacion == 1 ? cliente.RazonSocial : "",
                trade_name = "",
                names = cliente.IdTipoOrganizacion == 1 ? "" : $"{cliente.Nombre} {cliente.Apellido}",
                address = cliente.Direccion,
                email = cliente.Email,
                phone = cliente.Telefono,
                legal_organization_id = cliente.IdTipoOrganizacion.ToString(),
                tribute_id = cliente.IdTributo.ToString(),
                identification_document_id = cliente.IdTipoDocumentoIdentidad.ToString(),
                municipality_id = cliente.CiudadId,
            };
        }

        public static List<FactusItem> MapDetallesToFactusItems(List<DetalleFactura> detalles)
        {
            return detalles.Select(detalle => new FactusItem
            {
                scheme_id = "0",
                note = detalle.Producto.Descripcion ?? "",
                code_reference = detalle.Producto.Referencia ?? "",
                name = detalle.Producto.Nombre ?? "",
                quantity = detalle.Cantidad,
                discount_rate = detalle.ValorDescuento ?? 0,
                price = detalle.PrecioUnitario,
                tax_rate = "19.00", // IVA fijo al 19%
                unit_measure_id = 70,
                standard_code_id = 1,
                is_excluded = 0, // Siempre con IVA, no exento
                tribute_id = 1,
                withholding_taxes = new List<FactusWithholdingTax>(),
                mandate = null
            }).ToList();
        }

        public static FactusFacturaRequest MapFacturaToFactusRequest(
            Cliente cliente,
            List<DetalleFactura> detalles,
            string referencia,
            string observacion,
            string fechaVencimiento,
            string formaPago,
            string metodoPago,
            FactusEstablishment establecimiento,
            bool sendEmail // <-- Nuevo parámetro
        )
        {
            return new FactusFacturaRequest
            {
                numbering_range_id = 734,
                reference_code = referencia,
                observation = observacion ?? "",
                payment_form = formaPago,
                payment_due_date = fechaVencimiento,
                payment_method_code = metodoPago,
                operation_type = 10,
                send_email = sendEmail, // <-- Usar el parámetro
                establishment = establecimiento,
                customer = MapClienteToFactusCustomer(cliente),
                items = MapDetallesToFactusItems(detalles)
            };
        }

        // Algoritmo oficial DIAN para calcular el DV
        private static string CalcularDigitoVerificacion(string nit)
        {
            int[] pesos = { 71, 67, 59, 53, 47, 43, 41, 37, 29, 23, 19, 17, 13, 7, 3 };
            int suma = 0;
            int longitud = nit.Length;

            for (int i = 0; i < longitud; i++)
            {
                int digito = int.Parse(nit.Substring(i, 1));
                suma += digito * pesos[pesos.Length - longitud + i];
            }

            int residuo = suma % 11;
            int dv = residuo > 1 ? 11 - residuo : residuo;
            return dv.ToString();
        }
    }
}