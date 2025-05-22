using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Domain.Interfaces.SistemaInventario.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ProveedorService
{
    private readonly IProveedorRepository _proveedorRepository;

    public ProveedorService(IProveedorRepository proveedorRepository)
    {
        _proveedorRepository = proveedorRepository;
    }

    public async Task<IEnumerable<Proveedor>> ObtenerTodosAsync()
    {
        return await _proveedorRepository.ObtenerTodosAsync();
    }

    public async Task<Proveedor?> ObtenerPorIdAsync(Guid id)
    {
        return await _proveedorRepository.ObtenerPorIdAsync(id);
    }
    public async Task<Proveedor?> ObtenerPorNitAsync(string nit)
    {
        return await _proveedorRepository.ObtenerPorNitAsync(nit);
    }
    public async Task CrearAsync(Proveedor proveedor)
    {
        var existente = await _proveedorRepository.ObtenerPorNitAsync(proveedor.NIT);
        if (existente != null)
            throw new Exception("Ya existe un proveedor con ese NIT.");

        await _proveedorRepository.AgregarAsync(proveedor);
    }

    public async Task ActualizarAsync(Proveedor proveedor)
    {
        await _proveedorRepository.ActualizarAsync(proveedor);
    }

    public async Task EliminarAsync(Guid id)
    {
        await _proveedorRepository.EliminarAsync(id);
    }
}
