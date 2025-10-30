using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using virtualbook_backend.Data;
using virtualbook_backend.DTOs;
using virtualbook_backend.Models;

namespace virtualbook_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly VirtualBookDbContext _context;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(VirtualBookDbContext context, ILogger<UsuariosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        /// <param name="usuarioDto">Datos del usuario a registrar</param>
        /// <returns>Usuario registrado</returns>
        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UsuarioResponseDto>> RegistrarUsuario([FromBody] UsuarioRegisterDto usuarioDto)
        {
            try
            {
                // Verificar si el email ya existe
                var emailExiste = await _context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email);
                if (emailExiste)
                {
                    return BadRequest(new { mensaje = "El email ya está registrado" });
                }

                // Crear nuevo usuario
                var nuevoUsuario = new Usuario
                {
                    Nombre = usuarioDto.Nombre,
                    Email = usuarioDto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Password)
                };

                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuario registrado exitosamente: {Email}", nuevoUsuario.Email);

                // Retornar respuesta
                var respuesta = new UsuarioResponseDto
                {
                    Id = nuevoUsuario.Id,
                    Nombre = nuevoUsuario.Nombre,
                    Email = nuevoUsuario.Email
                };

                return CreatedAtAction(nameof(RegistrarUsuario), new { id = respuesta.Id }, respuesta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }
    }
}