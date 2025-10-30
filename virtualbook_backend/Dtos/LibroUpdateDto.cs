namespace virtualbook_backend.Dtos
{
    public class LibroUpdateDto
    {

        public int Id { get; set; }

        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string Genero { get; set; }
        public string Descripcion { get; set; }
    }
}
