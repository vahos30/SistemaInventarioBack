﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;

namespace SistemaInventario.Infrastructure.Repositories
{
    public class ProductoRepositoryProxy : IProductoRepository
    {
        private readonly IProductoRepository _repositorioReal;
        private readonly ILogger<ProductoRepositoryProxy> _logger; 

        public ProductoRepositoryProxy(IProductoRepository repositorioReal, ILogger<ProductoRepositoryProxy> logger)
        {
            _repositorioReal = repositorioReal;
            _logger = logger; // Asignamos el logger
        }

        public async Task AgregarAsync(Producto producto)
        {
            _logger.LogInformation("Agregando producto: {@Producto}", producto);
            await _repositorioReal.AgregarAsync(producto);
            _logger.LogInformation("Producto agregado correctamente.");
        }

        public async Task EliminarAsync(Guid id)
        {
            _logger.LogInformation("Eliminando producto con ID: {Id}", id);
            await _repositorioReal.EliminarAsync(id);
        }

        public async Task ActualizarAsync(Producto producto)
        {
            _logger.LogInformation("Actualizando producto: {@Producto}", producto);
            await _repositorioReal.ActualizarAsync(producto);
        }

        public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
        {
            _logger.LogInformation("Obteniendo todos los productos.");
            return await _repositorioReal.ObtenerTodosAsync();
        }

        public async Task<Producto?> ObtenerPorIdsync(Guid id)
        {
            _logger.LogInformation("Obteniendo producto con ID: {Id}", id);
            return await _repositorioReal.ObtenerPorIdsync(id);
        }

        public async Task<IEnumerable<Producto>> ObtenerProductosActivosAsync()
        {
            _logger.LogInformation("Obteniendo productos activos");
            var productosActivos = await _repositorioReal.ObtenerProductosActivosAsync();
            _logger.LogInformation("Se obtuvieron {Count} productos activos", productosActivos.Count());
            return productosActivos;
        }

        public async Task<bool> ExisteAsync(Guid productoId)
        {
            _logger.LogInformation("Verificando existencia del producto con ID: {ProductoId}", productoId);
            var existe = await _repositorioReal.ExisteAsync(productoId);
            _logger.LogInformation("Producto existe: {Existe}", existe);
            return existe;
        }

        public async Task<IEnumerable<Producto>> ObtenerProductosConDescripcionAsync()
        {
            _logger.LogInformation("Obteniendo productos con descripción");
            return await _repositorioReal.ObtenerProductosConDescripcionAsync();
        }


    }
}
