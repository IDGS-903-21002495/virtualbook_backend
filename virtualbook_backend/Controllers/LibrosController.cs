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
        [HttpGet("usuario/{id}")]
        public async Task<ActionResult<IEnumerable<LibrosListDto>>> ObtenerLibros(int id)
        {
            var libros = await _context.Libros
                .Where(l => l.UsuarioId == id) 
                .Select(l => new LibrosListDto
                {
                    Id = l.Id,
                    UsuarioId = l.UsuarioId,
                    Titulo = l.Titulo,
                    Autor = l.Autor,
                    Genero = l.Genero,
                    Descripcion = l.Descripcion
                })
                .ToListAsync();

            if (libros == null || !libros.Any())
            {
                return NotFound(new { mensaje = "El usuario no tiene libros registrados." });
            }

            return Ok(libros);
        }


        /// <summary>
        /// Obtiene un libro específico por su ID y el ID del usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario propietario</param>
        /// <param name="libroId">ID del libro</param>
        /// <returns>Datos del libro solicitado</returns>
        [HttpGet("usuario/{usuarioId}/libro/{libroId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LibrosListDto>> ObtenerLibroPorId(int usuarioId, int libroId)
        {
            try
            {
                if (usuarioId <= 0 || libroId <= 0)
                {
                    return BadRequest(new { mensaje = "IDs inválidos" });
                }

                var libro = await _context.Libros
                    .Where(l => l.Id == libroId && l.UsuarioId == usuarioId)
                    .Select(l => new LibrosListDto
                    {
                        Id = l.Id,
                        UsuarioId = l.UsuarioId,
                        Titulo = l.Titulo,
                        Autor = l.Autor,
                        Genero = l.Genero,
                        Descripcion = l.Descripcion
                    })
                    .FirstOrDefaultAsync();

                if (libro == null)
                {
                    return NotFound(new { mensaje = $"No se encontró un libro con ID {libroId} para el usuario {usuarioId}" });
                }

                _logger.LogInformation("Libro obtenido: {Id} - {Titulo} para usuario {UsuarioId}", libro.Id, libro.Titulo, usuarioId);

                return Ok(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el libro con ID {LibroId} para usuario {UsuarioId}", libroId, usuarioId);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }


        // Necesita el id del usuario para agregar un libro
        [HttpPost("usuario/{id}/libro")]
        public async Task<IActionResult> AgregarLibro(int id, [FromBody] LibroRegisterDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { mensaje = "Datos del libro no proporcionados" });
                }

                if (id <= 0)
                {
                    return BadRequest(new { mensaje = "ID de usuario inválido" });
                }

                // Verificar que el usuario existe
                var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == id);
                if (!usuarioExiste)
                {
                    return NotFound(new { mensaje = $"No se encontró el usuario con ID {id}" });
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

                _logger.LogInformation("Libro agregado exitosamente: {Titulo} para usuario {UsuarioId}", nuevoLibro.Titulo, id);

                var respuesta = new LibrosListDto
                {
                    Id = nuevoLibro.Id,
                    UsuarioId = nuevoLibro.UsuarioId,
                    Titulo = nuevoLibro.Titulo,
                    Autor = nuevoLibro.Autor,
                    Genero = nuevoLibro.Genero,
                    Descripcion = nuevoLibro.Descripcion
                };

                return CreatedAtAction(nameof(ObtenerLibroPorId), new { usuarioId = id, libroId = nuevoLibro.Id }, respuesta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar libro para usuario {UsuarioId}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }


        [HttpPut("usuario/{usuarioId}/libro/{libroId}")]
        public async Task<ActionResult<LibrosListDto>> ActualizarLibro(int usuarioId, int libroId, [FromBody] LibroUpdateDto dto)
        {
            try
            {
                if (dto == null || usuarioId <= 0 || libroId <= 0)
                {
                    return BadRequest(new { mensaje = "Datos inválidos o IDs no proporcionados" });
                }

                // Buscar el libro existente y verificar que pertenece al usuario
                var libro = await _context.Libros
                    .FirstOrDefaultAsync(l => l.Id == libroId && l.UsuarioId == usuarioId);

                if (libro == null)
                {
                    return NotFound(new { mensaje = $"No se encontró un libro con ID {libroId} para el usuario {usuarioId}" });
                }

                libro.Titulo = dto.Titulo;
                libro.Autor = dto.Autor;
                libro.Genero = dto.Genero;
                libro.Descripcion = dto.Descripcion;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Libro actualizado: {Id} - {Titulo} para usuario {UsuarioId}", libroId, libro.Titulo, usuarioId);

                var libroDto = new LibrosListDto
                {
                    Id = libro.Id,
                    UsuarioId = libro.UsuarioId,
                    Titulo = libro.Titulo,
                    Autor = libro.Autor,
                    Genero = libro.Genero,
                    Descripcion = libro.Descripcion
                };

                return Ok(libroDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar libro {LibroId} para usuario {UsuarioId}", libroId, usuarioId);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        [HttpDelete("usuario/{usuarioId}/libro/{libroId}")]
        public async Task<IActionResult> EliminarLibro(int usuarioId, int libroId)
        {
            try
            {
                if (usuarioId <= 0 || libroId <= 0)
                {
                    return BadRequest(new { mensaje = "IDs inválidos" });
                }

                // Verificar que el libro existe y pertenece al usuario
                var libro = await _context.Libros
                    .FirstOrDefaultAsync(l => l.Id == libroId && l.UsuarioId == usuarioId);

                if (libro == null)
                {
                    return NotFound(new { mensaje = $"No se encontró un libro con ID {libroId} para el usuario {usuarioId}" });
                }

                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Libro eliminado: {Id} - {Titulo} para usuario {UsuarioId}", libroId, libro.Titulo, usuarioId);

                return Ok(new { mensaje = $"El libro '{libro.Titulo}' ha sido eliminado correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar libro {LibroId} para usuario {UsuarioId}", libroId, usuarioId);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

    }
}
