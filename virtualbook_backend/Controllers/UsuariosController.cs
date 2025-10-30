using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using virtualbook_backend.Data;
using virtualbook_backend.DTOs;
using virtualbook_backend.Dtos;
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
        /// Obtiene la lista de todos los usuarios
        /// </summary>
        /// <returns>Lista de usuarios con nombre y email</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UsuariosListDto>>> ObtenerUsuarios()
        {
            try
            {
                var usuarios = await _context.Usuarios
                    .Select(u => new UsuariosListDto
                    {
                        Nombre = u.Nombre,
                        Email = u.Email
                    })
                    .ToListAsync();

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de usuarios");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
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


        /// <summary>
        /// Inicia sesión de usuario
        /// </summary>
        /// <param name="loginDto">Credenciales del usuario</param>
        /// <returns>Usuario autenticado</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UsuarioResponseDto>> Login([FromBody] UsuarioLoginDto loginDto)
        {
            try
            {
                if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                {
                    return BadRequest(new { mensaje = "Debe proporcionar email y contraseña" });
                }

                // Buscar usuario por email
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                if (usuario == null)
                {
                    return Unauthorized(new { mensaje = "Usuario no encontrado" });
                }

                // Verificar contraseña
                bool passwordValida = BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.Password);
                if (!passwordValida)
                {
                    return Unauthorized(new { mensaje = "Contraseña incorrecta" });
                }

                // (Opcional) Generar un token JWT si tu app lo requiere
                // De momento solo retornamos los datos del usuario

                var respuesta = new UsuarioResponseDto
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email
                };

                _logger.LogInformation("Usuario inició sesión: {Email}", usuario.Email);

                return Ok(new
                {
                    mensaje = "Inicio de sesión exitoso",
                    usuario = respuesta
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el inicio de sesión");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }


        /// <summary>
        /// Cierra la sesión del usuario actual
        /// </summary>
        /// <returns>Mensaje de confirmación</returns>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            _logger.LogInformation("Usuario cerró sesión");
            return Ok(new { mensaje = "Sesión cerrada exitosamente" });
        }

    }
}