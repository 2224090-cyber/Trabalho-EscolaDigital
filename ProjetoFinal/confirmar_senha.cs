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
    public partial class confirmar_senha : Form
    {
        public confirmar_senha()
        {
            InitializeComponent();
        }

        private void confirmar_senha_Load(object sender, EventArgs e)
        {
            textBox1.PasswordChar = '*';
            textBox2.PasswordChar = '*';
        }

        // ===================== BOTÃO 1: ATUALIZAR SENHA NO BANCO =====================
        private void button1_Click(object sender, EventArgs e)
        {
            string novaSenha = textBox1.Text;
            string confirmarSenha = textBox2.Text;

            if (string.IsNullOrWhiteSpace(novaSenha) || string.IsNullOrWhiteSpace(confirmarSenha))
            {
                MessageBox.Show("Preencha os dois campos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (novaSenha.Length < 6)
            {
                MessageBox.Show("A senha deve ter no mínimo 6 caracteres.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }

            if (novaSenha != confirmarSenha)
            {
                MessageBox.Show("As senhas não coincidem.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Focus();
                return;
            }

            // ✅ NOVO: Atualizar a senha diretamente no SQL Server usando o ID como String
            string query = "UPDATE Utilizadores SET Senha = @Senha WHERE Id = @Id";

            try
            {
                using (SqlConnection conexao = Database.GetConnection())
                {
                    using (SqlCommand comando = new SqlCommand(query, conexao))
                    {
                        comando.Parameters.AddWithValue("@Senha", novaSenha);
                        comando.Parameters.AddWithValue("@Id", Conta.Id); // Conta.Id tratado corretamente como string

                        conexao.Open();
                        int linhasAfetadas = comando.ExecuteNonQuery();

                        if (linhasAfetadas == 0)
                        {
                            MessageBox.Show("Não foi possível encontrar a sua conta para atualizar a senha.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar a senha na Base de Dados: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Atualiza também na memória local por segurança
            Conta.Senha = novaSenha;

            MessageBox.Show("Senha alterada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Hide();
            using (var Form1 = new Form1())
            {
                Form1.ShowDialog();
            }

            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void textBox2_TextChanged(object sender, EventArgs e) { }

        // ===================== BOTÃO 3: CANCELAR / VOLTAR =====================
        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var Form1 = new Form1())
            {
                Form1.ShowDialog();
            }
            this.Close();
        }

        // ===================== MOSTRAR/OCULTAR NOVA SENHA (textBox1) =====================
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.PasswordChar == '*')
            {
                textBox1.PasswordChar = '\0'; // mostra a senha
            }
            else
            {
                textBox1.PasswordChar = '*'; // volta a ocultar
            }
        }

        // ===================== MOSTRAR/OCULTAR CONFIRMAR SENHA (textBox2) =====================
        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox2.PasswordChar == '*')
            {
                textBox2.PasswordChar = '\0'; // mostra a senha
            }
            else
            {
                textBox2.PasswordChar = '*'; // volta a ocultar
            }
        }
    }
}