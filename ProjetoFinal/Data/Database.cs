using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace Guryflix.Data
{
    public class Database
    {
        // Configurações da base de dados
        private static string servidor = "localhost";
        private static string baseDados = "guryflix";
        private static string utilizadorBd = "root";
        private static string palavraPasseBd = ""; 

        private static string stringConexao = $"Server={servidor};Database={baseDados};Uid={utilizadorBd};Pwd={palavraPasseBd};";

        public static MySqlConnection ObterConexao()
        {
            try
            {
                return new MySqlConnection(stringConexao);
            }


            catch (MySqlException ex)
            {
                MessageBox.Show("Erro de MySQL: " + ex.Message, "Erro de Base de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            catch (Exception ex)

            {
                MessageBox.Show("Erro ao criar conexão: " + ex.Message, "Erro de Base de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            
        }

        public static void TestarConexao()
        {
            using (MySqlConnection conexao = ObterConexao())
            {
                try
                {
                    conexao.Open();
                    MessageBox.Show("Conexão à base de dados 'guryflix' estabelecida com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Falha ao ligar à base de dados: " + ex.Message, "Erro de Conexão", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
