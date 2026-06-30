using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Horazon_Bank__projetoFinal
{
    public partial class alterar_senha : Form
    {
        public alterar_senha()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            string emailDigitado = textBox1.Text.Trim();

            
            if (string.IsNullOrWhiteSpace(emailDigitado))
            {
                MessageBox.Show("Digite o email da sua conta.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }

            
            string query = "SELECT Id, Nome FROM Utilizadores WHERE Email = @Email";
            bool emailExiste = false;

            try
            {
                using (SqlConnection conexao = Database.GetConnection())
                {
                    using (SqlCommand comando = new SqlCommand(query, conexao))
                    {
                        comando.Parameters.AddWithValue("@Email", emailDigitado);
                        conexao.Open();

                        using (SqlDataReader reader = comando.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                emailExiste = true;

                               
                                Conta.Id = reader["Id"].ToString();
                                Conta.Email = emailDigitado;
                                Conta.Nome = reader["Nome"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao consultar a base de dados: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            if (!emailExiste)
            {
                MessageBox.Show("Esse email não tem conta criada no Horizon Bank.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }

     
            Conta.CodigoVerificacao = Conta.GerarCodigoVerificacao();

            email emailBanco = new email(
                "smtp.gmail.com",
                587,
                "horizonbank.f1@gmail.com",
                "liog nhuo xddf jpwk" 
            );

           
            Cursor.Current = Cursors.WaitCursor;
            bool enviado = emailBanco.EnviarCodigoVerificacao(Conta.Email, Conta.CodigoVerificacao);
            Cursor.Current = Cursors.Default;

            if (!enviado)
            {
                MessageBox.Show("Não foi possível enviar o código de verificação. Tente novamente.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Um código de verificação foi enviado para o seu email.", "Código Enviado",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

           
            this.Hide();
            using (var verificacao = new verificacao_de_conta(ModoVerificacao.ResetSenha))
            {
                verificacao.ShowDialog();
            }

            this.Close();
        }

       
        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var Form1 = new Form1())
            {
                Form1.ShowDialog();
            }
            this.Close(); 
        }
    }
}