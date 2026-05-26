using Guryflix.Data;
using Guryflix.Models;
using MySql.Data.MySqlClient;
using System;

namespace Guryflix.Repositories
{
    public class UtilizadorRepository
    {
        public Utilizador IniciarSessao(string nomeUtilizadorOuEmail, string palavraPasse)
        {
            using (MySqlConnection conexao = Database.ObterConexao())
            {
                string consulta = "SELECT * FROM Utilizadores WHERE (username = @valor OR email = @valor) AND password = @passe";
                MySqlCommand comando = new MySqlCommand(consulta, conexao);
                comando.Parameters.AddWithValue("@valor", nomeUtilizadorOuEmail);
                comando.Parameters.AddWithValue("@passe", palavraPasse);

                try
                {
                    conexao.Open();
                    using (MySqlDataReader leitor = comando.ExecuteReader())
                    {
                        if (leitor.Read())
                        {
                            return new Utilizador
                            {
                                Id = Convert.ToInt32(leitor["id"]),
                                NomeUtilizador = leitor["username"].ToString(),
                                Email = leitor["email"].ToString()
                            };
                        }
                    }
                }
                catch (Exception) { /* Tratar erro ou fazer log */ }
            }
            return null;
        }

        public bool RegistarUtilizador(Utilizador utilizador)
        {
            using (MySqlConnection conexao = Database.ObterConexao())
            {
                string consulta = "INSERT INTO Utilizadores (username, email, password) VALUES (@nome, @email, @passe)";
                MySqlCommand comando = new MySqlCommand(consulta, conexao);
                comando.Parameters.AddWithValue("@nome", utilizador.NomeUtilizador);
                comando.Parameters.AddWithValue("@email", utilizador.Email);
                comando.Parameters.AddWithValue("@passe", utilizador.PalavraPasse);

                try
                {
                    conexao.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
                catch (Exception) { return false; }
            }
        }
    }
}
