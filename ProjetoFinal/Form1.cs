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

            
            if (string.IsNullOrWhiteSpace(emailDigitado) || string.IsNullOrWhiteSpace(senhaDigitada))
            {
                MessageBox.Show("Preencha o email e a senha.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

           
            using (SqlConnection conexao = Database.GetConnection())
            {
                try
                {
                    conexao.Open();

                    
                    string query = "SELECT Id, Senha FROM Utilizadores WHERE Email = @Email";

                    using (SqlCommand comando = new SqlCommand(query, conexao))
                    {
                        comando.Parameters.AddWithValue("@Email", emailDigitado);

                        using (SqlDataReader leitor = comando.ExecuteReader())
                        {
                           
                            if (!leitor.Read())
                            {
                                MessageBox.Show("O email introduzido não se encontra registado.", "Erro de Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                           
                            string senhaBanco = leitor["Senha"].ToString();

                            
                            string utilizadorId = leitor["Id"].ToString();


                            if (senhaDigitada != senhaBanco)
                            {
                                MessageBox.Show("Senha incorreta. Tente novamente.", "Erro de Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            
                            MessageBox.Show("Login efetuado com sucesso!", "Bem-vindo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                         
                            Conta.Id = utilizadorId;
                            Conta.Email = emailDigitado;

                           
                            this.Hide();
                            using (var menuPrincipalForm = new menu_principal())
                            {
                                menuPrincipalForm.ShowDialog();
                            }
                            this.Show(); 
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

      
        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox2.PasswordChar == '*')
            {
                textBox2.PasswordChar = '\0'; 
                button5.Text = "";
            }
            else
            {
                textBox2.PasswordChar = '*'; 
                button5.Text = "";
            }
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}