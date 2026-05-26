namespace Guryflix.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public Categoria() { }

        public Categoria(int id, string nome)
        {
            Id = id;
            Nome = nome;
        }

        public override string ToString()
        {
            return Nome;
        }
    }
}
