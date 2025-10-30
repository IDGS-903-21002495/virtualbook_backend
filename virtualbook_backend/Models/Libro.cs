using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace virtualbook_backend.Models
{
    public class Libro
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; }


        [Required]
        [MaxLength(100)]
        public string Autor { get; set; }

        [Required]
        [MaxLength(100)]
        public string Genero { get; set; }

        [MaxLength(500)]
        public string Descripcion { get; set; }

        // Foreign Key
        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }
    }
}
