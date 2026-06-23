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

            // Verificar se existe uma conta com este email
            if (string.IsNullOrEmpty(Conta.Email) || emailDigitado != Conta.Email)
            {
                MessageBox.Show("Esse email não tem conta criada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }

            // Gerar e enviar código de verificação
            Conta.CodigoVerificacao = Conta.GerarCodigoVerificacao();

            email emailBanco = new email(
                "smtp.gmail.com",
                587,
                "horizonbank.f1@gmail.com",
                "liog nhuo xddf jpwk"
            );

            bool enviado = emailBanco.EnviarCodigoVerificacao(Conta.Email, Conta.CodigoVerificacao);

            if (!enviado)
            {
                MessageBox.Show("Não foi possível enviar o código de verificação. Tente novamente.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Um código de verificação foi enviado para o seu email.", "Código Enviado",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Abrir verificacao_de_conta no modo de reset de senha
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
        }
    }
}
