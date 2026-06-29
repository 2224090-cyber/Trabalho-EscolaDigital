using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Horazon_Bank__projetoFinal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '*';
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string emailDigitado = textBox1.Text.Trim();
            string senhaDigitada = textBox2.Text;

            // 1. Validar campos vazios (Front-end)
            if (string.IsNullOrWhiteSpace(emailDigitado) || string.IsNullOrWhiteSpace(senhaDigitada))
            {
                MessageBox.Show("Preencha o email e a senha.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. Ligar à Base de Dados para validar as credenciais
            using (SqlConnection conexao = Database.GetConnection())
            {
                try
                {
                    conexao.Open();

                    // ✅ ALTERADO: Mudamos 'SenhaHash' para 'Senha' para bater certo com a tua tabela
                    string query = "SELECT Id, Senha FROM Utilizadores WHERE Email = @Email";

                    using (SqlCommand comando = new SqlCommand(query, conexao))
                    {
                        comando.Parameters.AddWithValue("@Email", emailDigitado);

                        using (SqlDataReader leitor = comando.ExecuteReader())
                        {
                            // Caso o email NÃO exista na base de dados
                            if (!leitor.Read())
                            {
                                MessageBox.Show("O email introduzido não se encontra registado.", "Erro de Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            // ✅ ALTERADO: Lemos a coluna 'Senha' que vem da base de dados
                            string senhaBanco = leitor["Senha"].ToString();

                            // Tratamento seguro para converter o ID (evita erros caso o ID seja armazenado como string grande)
                            string utilizadorId = leitor["Id"].ToString();

                            // Caso a senha esteja incorreta
                            if (senhaDigitada != senhaBanco)
                            {
                                MessageBox.Show("Senha incorreta. Tente novamente.", "Erro de Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            // Se chegou aqui, os dados estão corretos! 
                            MessageBox.Show("Login efetuado com sucesso!", "Bem-vindo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Aloca o ID e Email na classe global Conta para o menu saber quem entrou
                            Conta.Id = utilizadorId;
                            Conta.Email = emailDigitado;

                            // Abrir o Menu Principal
                            this.Hide();
                            using (var menuPrincipalForm = new menu_principal())
                            {
                                menuPrincipalForm.ShowDialog();
                            }
                            this.Show(); // Mostra o login novamente se o menu principal fechar
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao ligar ao servidor: " + ex.Message, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var criar_Conta = new criar_conta())
            {
                criar_Conta.ShowDialog();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var alterarSenha = new alterar_senha())
            {
                alterarSenha.ShowDialog();
            }
        }

        // ===================== MOSTRAR/OCULTAR SENHA =====================

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox2.PasswordChar == '*')
            {
                textBox2.PasswordChar = '\0'; // mostra a senha
                button5.Text = "";
            }
            else
            {
                textBox2.PasswordChar = '*'; // volta a ocultar
                button5.Text = "";
            }
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}