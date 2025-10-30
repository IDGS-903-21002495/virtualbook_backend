namespace virtualbook_backend.Dtos
{
    public class LibroRegisterDto
    {
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string Genero { get; set; }
        public string Descripcion { get; set; }
        public int UsuarioId { get; set; }
    }
}
