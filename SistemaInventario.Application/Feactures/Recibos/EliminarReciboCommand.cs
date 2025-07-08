using MediatR;
using System;

public class EliminarReciboCommand : IRequest
{
    public Guid Id { get; set; }
    public EliminarReciboCommand(Guid id) => Id = id;
}