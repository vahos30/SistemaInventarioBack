using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface INotaCreditoRepository
{
    Task AgregarAsync(NotaCredito notaCredito);
    Task<NotaCredito?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<NotaCredito>> ObtenerNotasCreditoAsync();
}