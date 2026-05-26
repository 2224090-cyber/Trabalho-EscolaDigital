using System;

namespace Guryflix.Models
{
    public class Filme
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string UrlVideo { get; set; }
        public string Thumbnail { get; set; }
        public int Ano { get; set; }
        public double Rating { get; set; }
        public int CategoriaId { get; set; }

        public Filme() { }
    }
}
