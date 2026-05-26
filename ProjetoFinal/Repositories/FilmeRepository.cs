using Guryflix.Data;
using Guryflix.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Guryflix.Repositories
{
    public class FilmeRepository
    {
        public List<Filme> ListarTodosOsFilmes()
        {
            List<Filme> listaFilmes = new List<Filme>();
            using (MySqlConnection conexao = Database.ObterConexao())
            {
                string consulta = "SELECT * FROM Filmes";
                MySqlCommand comando = new MySqlCommand(consulta, conexao);

                try
                {
                    conexao.Open();
                    using (MySqlDataReader leitor = comando.ExecuteReader())
                    {
                        while (leitor.Read())
                        {
                            listaFilmes.Add(new Filme
                            {
                                Id = Convert.ToInt32(leitor["id"]),
                                Titulo = leitor["titulo"].ToString(),
                                Descricao = leitor["descricao"].ToString(),
                                UrlVideo = leitor["url_video"].ToString(),
                                Thumbnail = leitor["thumbnail"].ToString(),
                                Ano = Convert.ToInt32(leitor["ano"]),
                                Rating = Convert.ToDouble(leitor["rating"]),
                                CategoriaId = Convert.ToInt32(leitor["categoria_id"])
                            });
                        }
                    }
                }
                catch (Exception) { /* Tratar erro */ }
            }
            return listaFilmes;
        }
    }
}
