using MediatR;
using System;

public class AnularFacturaCommand : IRequest
{
    public Guid FacturaId { get; set; }
    public string Motivo { get; set; } = string.Empty;
}