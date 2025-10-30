using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using virtualbook_backend.Data;
using virtualbook_backend.Dtos;
using virtualbook_backend.Models;

namespace virtualbook_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly VirtualBookDbContext _context;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(VirtualBookDbContext context, ILogger<LibrosController> logger)
        {
            _context = context;
            _logger = logger;
        }



        // se necisita el id del usuario que inicio sesion para mostrar solos los libros que el registro
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<LibrosListDto>>> ObtenerLibros(int id)
        {
            var libros = await _context.Libros
                .Where(l => l.UsuarioId == id) 
                .Select(l => new LibrosListDto
                {
                    Id = l.Id,
                    UsuarioId = l.Id,
                    Titulo = l.Titulo,
                    Autor = l.Autor,
                    Genero = l.Genero,
                    Descripcion = l.Descripcion
                })
                .ToListAsync();

            if (libros == null || !libros.Any())
            {
                return NotFound($"El usuario  no tiene libros registrados.");
            }

            return Ok(libros);
        }


        // Necesita el id del libro que se desea actualizar
        [HttpPost("{id}/libro")]
        public async Task<IActionResult> AgregarLibro(int id, [FromBody] LibroRegisterDto dto)
        {
            if (dto == null) 
            {
                return BadRequest( dto);
            }

            var nuevoLibro = new Libro
            {
                Titulo = dto.Titulo,
                Autor = dto.Autor,
                Genero = dto.Genero,
                Descripcion = dto.Descripcion,
                UsuarioId = id
            };

            _context.Libros.Add(nuevoLibro);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Libro agregado exitosamente: {Titulo}", nuevoLibro.Titulo);

            return Ok(nuevoLibro);
        }


        [HttpPut]
        public async Task<ActionResult<LibrosListDto>> ActualizarLibro([FromBody] LibroUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
                return BadRequest("Datos inválidos o ID no proporcionado.");

            // Buscar el libro existente
            var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == dto.Id);
            if (libro == null)
                return NotFound($"No se encontró un libro con ID {dto.Id}.");

            libro.Titulo = dto.Titulo;
            libro.Autor = dto.Autor;
            libro.Genero = dto.Genero;
            libro.Descripcion = dto.Descripcion;

            await _context.SaveChangesAsync();

            var libroDto = new LibrosListDto
            {
                Id = libro.Id,
                Titulo = libro.Titulo,
                Autor = libro.Autor,
                Genero = libro.Genero,
                Descripcion = libro.Descripcion
            };

            return Ok(libroDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarLibro(int id)
        {
            if (id <= 0)
                return BadRequest("ID inválido.");

            var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
            if (libro == null)
                return NotFound($"No se encontró un libro con ID {id}.");

            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"El libro  ha sido eliminado correctamente." });
        }

    }
}
