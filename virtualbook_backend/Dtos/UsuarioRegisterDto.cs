using System.ComponentModel.DataAnnotations;

namespace virtualbook_backend.Dtos
{
    public class UsuarioRegisterDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]        
        [EmailAddress(ErrorMessage = "El formato del email no es v�lido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contrase�a es obligatoria")]
        [MinLength(6, ErrorMessage = "La contrase�a debe tener al menos 6 caracteres")]
        public string Password { get; set; }
    }
}