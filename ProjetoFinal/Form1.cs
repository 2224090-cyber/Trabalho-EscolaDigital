using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            // Validar campos vazios
            if (string.IsNullOrWhiteSpace(emailDigitado) || string.IsNullOrWhiteSpace(senhaDigitada))
            {
                MessageBox.Show("Preencha o email e a senha.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Verificar se existe alguma conta criada
            if (string.IsNullOrEmpty(Conta.Email))
            {
                MessageBox.Show("Nenhuma conta encontrada. Crie uma conta primeiro.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Comparar email e senha com os dados guardados
            if (emailDigitado != Conta.Email || senhaDigitada != Conta.Senha)
            {
                MessageBox.Show("Email ou senha incorretos.", "Erro de Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Login correto
            this.Hide();
            using (var menu_principal = new menu_principal())
            {
                menu_principal.ShowDialog();
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
    }
}