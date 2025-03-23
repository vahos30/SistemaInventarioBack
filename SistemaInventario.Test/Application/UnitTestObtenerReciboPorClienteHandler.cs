using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SistemaInventario.Application.Feactures.Recibos;
using SistemaInventario.Application.Feactures.Reportes;
using SistemaInventario.Application.Mapping;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;

namespace SistemaInventario.Test.Application
{
    [TestClass]
    public class UnitTestObtenerReciboPorClienteHandler
    {
        private Mock<IReciboRepository> _reciboRepositoryMock;
        private IMapper _mapper;
        private ObtenerRecibosPorClienteHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            //configuramos el mock para IReciboRepository
            _reciboRepositoryMock = new Mock<IReciboRepository>();
            //configuramos AutoMapper usando el MappingProfile de la aplicacion
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();
            //creamos la instancia del handler inyectandq el mock y el mapper
            _handler = new ObtenerRecibosPorClienteHandler(_reciboRepositoryMock.Object, _mapper);
        }

        [TestMethod]
        public async Task Handle_ValidQuery_ShouldReturnRecibos()
        {
            var clienteId = Guid.NewGuid();
            var recibos = new List<Recibo>
            {
                new Recibo { Id = Guid.NewGuid(), ClienteId = clienteId, Fecha = DateTime.UtcNow  },
                new Recibo { Id = Guid.NewGuid(), ClienteId = clienteId, Fecha = DateTime.UtcNow }
            };

            _reciboRepositoryMock.Setup(repo => repo.ObtenerRecibosPorClientesAsync(clienteId)).ReturnsAsync(recibos);

            var query = new ObtenerRecibosPorClienteQuery (clienteId);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());

        }
    }
}
