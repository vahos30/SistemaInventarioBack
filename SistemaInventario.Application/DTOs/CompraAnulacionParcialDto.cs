public class CompraAnulacionParcialDto
{
    public Guid CompraId { get; set; }
    public List<DetalleAnulacionDto> Detalles { get; set; }
}

public class DetalleAnulacionDto
{
    public Guid ProductoId { get; set; }
    public int CantidadAAnular { get; set; }
    public string MotivoDevolucion { get; set; } // <-- NUEVO
}