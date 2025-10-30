using System.ComponentModel.DataAnnotations;

namespace virtualbook_backend.Models
{
    public class Usuario
    {
        //Entidad usuario
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        // Relación con libros
        public ICollection<Libro> Libros { get; set; }
    }
}
