using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Application.Feactures.Clientes;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Infrastructure.Persistence;

namespace SistemaInventario.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ClientesController(AppDbContext context, IMapper mapper, IMediator mediator)
        {
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
        {
            var clientes = await _context.Clientes.ToListAsync();
            return Ok(_mapper.Map<List<ClienteDto>>(clientes));
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> GetCliente(Guid id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Recibos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null) return NotFound();
            return Ok(_mapper.Map<ClienteDto>(cliente));
        }

        // GET: api/Clientes/NumeroDocumento/

        [HttpGet("por-documento/{numeroDocumento}")]
        public async Task<ActionResult<ClienteDto>> GetClientePorNumeroDocumento(string numeroDocumento)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Recibos)
                .FirstOrDefaultAsync(c => c.NumeroDocumento == numeroDocumento);
            if (cliente == null) return NotFound();
            return Ok(_mapper.Map<ClienteDto>(cliente));
        }


        // PUT: api/Clientes/5

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(Guid id, [FromBody] ClienteDto clienteDto)
        {
            if (id != clienteDto.Id) return BadRequest();

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            _mapper.Map(clienteDto, cliente); // ✅ Actualiza la entidad desde el DTO
            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<ClienteDto>> PostCliente([FromBody] CrearClienteCommand command)
        {
            var clienteDto = await _mediator.Send(command);
            return CreatedAtAction("GetCliente", new { id = clienteDto.Id }, clienteDto);
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(Guid id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClienteExists(Guid id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}
