using System;

namespace Guryflix.Models
{
    public class Utilizador
    {
        public int Id { get; set; }
        public string NomeUtilizador { get; set; }
        public string Email { get; set; }
        public string PalavraPasse { get; set; }
        public DateTime DataCriacao { get; set; }

        public Utilizador() { }

        public Utilizador(int id, string nomeUtilizador, string email, string palavraPasse)
        {
            Id = id;
            NomeUtilizador = nomeUtilizador;
            Email = email;
            PalavraPasse = palavraPasse;
        }
    }
}
