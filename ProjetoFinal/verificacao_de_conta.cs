using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;


namespace Horazon_Bank__projetoFinal
{
    public partial class verificacao_de_conta : Form
    {
        private ModoVerificacao modo;

        // Construtor padrão (mantém compatibilidade com o fluxo de criar conta)
        public verificacao_de_conta()
        {
            InitializeComponent();
            modo = ModoVerificacao.CriarConta;
        }

        // Construtor com modo (usado no fluxo de reset de senha)
        public verificacao_de_conta(ModoVerificacao modoVerificacao)
        {
            InitializeComponent();
            modo = modoVerificacao;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void verificacão_de_conta_Load(object sender, EventArgs e)
        {
            textBox6.Focus(); // primeira caixa a receber foco
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        // ===================== AVANÇO/RECUO AUTOMÁTICO =====================

        private void LimitarParaUmDigito(TextBox caixaAtual, TextBox proximaCaixa)
        {
            string texto = caixaAtual.Text;

            // Remove qualquer coisa que não seja número
            string apenasNumeros = Regex.Replace(texto, @"[^0-9]", "");

            // Mantém só o último caractere digitado (caso cole texto longo)
            if (apenasNumeros.Length > 1)
            {
                apenasNumeros = apenasNumeros.Substring(apenasNumeros.Length - 1);
            }

            if (caixaAtual.Text != apenasNumeros)
            {
                caixaAtual.Text = apenasNumeros;
                caixaAtual.SelectionStart = caixaAtual.Text.Length;
            }

            // Se preencheu o dígito, avança para a próxima caixa
            if (apenasNumeros.Length == 1 && proximaCaixa != null)
            {
                proximaCaixa.Focus();
                proximaCaixa.SelectAll();
            }
        }

        private void VoltarComBackspace(object sender, KeyEventArgs e, TextBox caixaAnterior)
        {
            TextBox caixaAtual = (TextBox)sender;

            if (e.KeyCode == Keys.Back && caixaAtual.Text.Length == 0 && caixaAnterior != null)
            {
                caixaAnterior.Focus();
                caixaAnterior.SelectAll();
            }
        }

        // ===================== ORDEM: textBox6 → textBox2 → textBox1 → textBox3 → textBox4 → textBox5 =====================

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            LimitarParaUmDigito(textBox6, textBox2);
        }

        private void textBox6_KeyDown(object sender, KeyEventArgs e)
        {
            VoltarComBackspace(sender, e, null); // primeira caixa, não há anterior
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            LimitarParaUmDigito(textBox2, textBox1);
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            VoltarComBackspace(sender, e, textBox6);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            LimitarParaUmDigito(textBox1, textBox3);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            VoltarComBackspace(sender, e, textBox2);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            LimitarParaUmDigito(textBox3, textBox4);
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            VoltarComBackspace(sender, e, textBox1);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            LimitarParaUmDigito(textBox4, textBox5);
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            VoltarComBackspace(sender, e, textBox3);
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            LimitarParaUmDigito(textBox5, null); // última caixa
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            VoltarComBackspace(sender, e, textBox4);
        }

        // ===================== CONFIRMAR CÓDIGO (button1) =====================

        private void button1_Click(object sender, EventArgs e)
        {
            string codigoDigitado = textBox6.Text + textBox2.Text + textBox1.Text +
                                     textBox3.Text + textBox4.Text + textBox5.Text;

            if (codigoDigitado.Length < 6)
            {
                MessageBox.Show("Preencha todos os campos do código.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (codigoDigitado != Conta.CodigoVerificacao)
            {
                MessageBox.Show("Código incorreto. Tente novamente.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

                textBox6.Clear();
                textBox2.Clear();
                textBox1.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                textBox6.Focus();
                return;
            }

            // Código correto
            Conta.CodigoVerificacao = ""; // já não é mais necessário
            this.DialogResult = DialogResult.OK;

            if (modo == ModoVerificacao.CriarConta)
            {
                this.Hide();
                using (var Documentos = new documentos())
                {
                    Documentos.ShowDialog();
                }
            }
            else if (modo == ModoVerificacao.ResetSenha)
            {
                this.Hide();
                using (var confirmarSenha = new confirmar_senha())
                {
                    confirmarSenha.ShowDialog();
                }
            }

            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}