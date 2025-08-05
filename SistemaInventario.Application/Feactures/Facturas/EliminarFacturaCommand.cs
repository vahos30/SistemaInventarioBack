using MediatR;
using System;

public class EliminarFacturaCommand : IRequest
{
    public Guid Id { get; set; }
    public EliminarFacturaCommand(Guid id) => Id = id;
}