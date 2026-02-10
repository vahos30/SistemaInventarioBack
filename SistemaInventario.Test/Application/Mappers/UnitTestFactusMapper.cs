using Microsoft.VisualStudio.TestTools.UnitTesting;
using SistemaInventario.Application.Mappers;
using SistemaInventario.Domain.Entities;
using System.Collections.Generic;

namespace SistemaInventario.Test.Application.Mappers
{
    [TestClass]
    public class UnitTestFactusMapper
    {
        [TestMethod]
        public void MapClienteToFactusCustomer_NITConDV_ExtraeNitYDV()
        {
            var cliente = new Cliente
            {
                TipoDocumento = "NIT",
                NumeroDocumento = "900123456-7",
                IdTipoOrganizacion = 1,
                RazonSocial = "Empresa S.A.",
                Direccion = "Calle 1",
                Email = "test@empresa.com",
                Telefono = "1234567",
                IdTributo = 2,
                IdTipoDocumentoIdentidad = 3,
                CiudadId = 05001
            };

            var result = FactusMapper.MapClienteToFactusCustomer(cliente);

            Assert.AreEqual("900123456", result.identification);
            Assert.AreEqual("7", result.dv);
            Assert.AreEqual("Empresa S.A.", result.company);
            Assert.AreEqual("Calle 1", result.address);
            Assert.AreEqual("test@empresa.com", result.email);
            Assert.AreEqual("1234567", result.phone);
            Assert.AreEqual("1", result.legal_organization_id);
            Assert.AreEqual("2", result.tribute_id);
            Assert.AreEqual("3", result.identification_document_id);
            Assert.AreEqual(05001, result.municipality_id);
        }

        [TestMethod]
        public void MapClienteToFactusCustomer_NITSinDV_CalculaDV()
        {
            var cliente = new Cliente
            {
                TipoDocumento = "NIT",
                NumeroDocumento = "900123456",
                IdTipoOrganizacion = 2,
                Nombre = "Juan",
                Apellido = "Pérez",
                Direccion = "Calle 2",
                Email = "jp@correo.com",
                Telefono = "7654321",
                IdTributo = 4,
                IdTipoDocumentoIdentidad = 5,
                CiudadId = 05002
            };

            var result = FactusMapper.MapClienteToFactusCustomer(cliente);

            Assert.AreEqual("900123456", result.identification);
            Assert.IsFalse(string.IsNullOrEmpty(result.dv));
            Assert.AreEqual("Juan Pérez", result.names);
            Assert.AreEqual("", result.company);
        }

        [TestMethod]
        public void MapClienteToFactusCustomer_CC_SinDV()
        {
            var cliente = new Cliente
            {
                TipoDocumento = "CC",
                NumeroDocumento = "123456789",
                IdTipoOrganizacion = 2,
                Nombre = "Ana",
                Apellido = "Gómez",
                Direccion = "Calle 3",
                Email = "ana@correo.com",
                Telefono = "1112223",
                IdTributo = 6,
                IdTipoDocumentoIdentidad = 7,
                CiudadId = 05003
            };

            var result = FactusMapper.MapClienteToFactusCustomer(cliente);

            Assert.AreEqual("123456789", result.identification);
            Assert.AreEqual("", result.dv);
            Assert.AreEqual("Ana Gómez", result.names);
        }

        [TestMethod]
        public void MapDetallesToFactusItems_MapeaCorrectamente()
        {
            var detalles = new List<DetalleFactura>
            {
                new DetalleFactura
                {
                    Producto = new Producto { Descripcion = "Desc", Referencia = "Ref", Nombre = "Prod" },
                    Cantidad = 2,
                    ValorDescuento = 5,
                    PrecioUnitario = 100
                }
            };

            var result = FactusMapper.MapDetallesToFactusItems(detalles);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Desc", result[0].note);
            Assert.AreEqual("Ref", result[0].code_reference);
            Assert.AreEqual("Prod", result[0].name);
            Assert.AreEqual(2, result[0].quantity);
            Assert.AreEqual(5, result[0].discount_rate);
            Assert.AreEqual(100, result[0].price);
            Assert.AreEqual("19.00", result[0].tax_rate);
            Assert.AreEqual(70, result[0].unit_measure_id);
            Assert.AreEqual(1, result[0].standard_code_id);
            Assert.AreEqual(0, result[0].is_excluded);
            Assert.AreEqual(1, result[0].tribute_id);
        }

        [TestMethod]
        public void MapFacturaToFactusRequest_MapeaTodosLosCampos()
        {
            var cliente = new Cliente
            {
                TipoDocumento = "CC",
                NumeroDocumento = "123456789",
                IdTipoOrganizacion = 2,
                Nombre = "Ana",
                Apellido = "Gómez",
                Direccion = "Calle 3",
                Email = "ana@correo.com",
                Telefono = "1112223",
                IdTributo = 6,
                IdTipoDocumentoIdentidad = 7,
                CiudadId = 05003
            };
            var detalles = new List<DetalleFactura>
            {
                new DetalleFactura
                {
                    Producto = new Producto { Descripcion = "Desc", Referencia = "Ref", Nombre = "Prod" },
                    Cantidad = 2,
                    ValorDescuento = 5,
                    PrecioUnitario = 100
                }
            };
            var establecimiento = new FactusEstablishment();
            var result = FactusMapper.MapFacturaToFactusRequest(
                cliente,
                detalles,
                "REF123",
                "Observación",
                "2026-02-10",
                "1",
                "2",
                establecimiento,
                true
            );

            Assert.AreEqual(755, result.numbering_range_id);
            Assert.AreEqual("REF123", result.reference_code);
            Assert.AreEqual("Observación", result.observation);
            Assert.AreEqual("1", result.payment_form);
            Assert.AreEqual("2026-02-10", result.payment_due_date);
            Assert.AreEqual("2", result.payment_method_code);
            Assert.AreEqual(10, result.operation_type);
            Assert.IsTrue(result.send_email);
            Assert.AreEqual(establecimiento, result.establishment);
            Assert.IsNotNull(result.customer);
            Assert.AreEqual(1, result.items.Count);
        }
    }
}
