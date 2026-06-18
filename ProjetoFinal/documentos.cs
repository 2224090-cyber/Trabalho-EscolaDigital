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


namespace Horazon_Bank__projetoFinal
{
    public partial class documentos : Form
    {
        private string emailUsuario = "";
        private string nomeUsuario = "";

        public documentos()
        {
            InitializeComponent();
        }

        public documentos(string email, string nome)
        {
            InitializeComponent();
            emailUsuario = email;
            nomeUsuario = nome;
        }

        // Validar Cartão de Cidadão (formato: 12345678 AB ou sem espaço)
        private bool ValidarCartaoCidadao(string cartao)
        {
            if (string.IsNullOrWhiteSpace(cartao))
                return false;

            // Remover espaços
            string cartaoLimpo = cartao.Replace(" ", "").ToUpper();

            // Formato: 8 dígitos + 2 letras (total 10 caracteres)
            return Regex.IsMatch(cartaoLimpo, @"^\d{8}[A-Z]{2}$");
        }

        // Validar Passaporte (formato flexível: letras e números)
        private bool ValidarPassaporte(string passaporte)
        {
            if (string.IsNullOrWhiteSpace(passaporte))
                return false;

            // Passaporte: 1-2 letras + 6-7 números
            // Exemplo: AB123456 ou A1234567
            return Regex.IsMatch(passaporte, @"^[A-Z]{1,2}\d{6,7}$");
        }

        // Validar NIF - Número de Identificação Fiscal (9 dígitos)
        private bool ValidarNIF(string nif)
        {
            if (string.IsNullOrWhiteSpace(nif))
                return false;

            // NIF deve ter 9 dígitos
            if (!Regex.IsMatch(nif, @"^\d{9}$"))
                return false;

            // NIF português começa com: 1, 2, 5, 6, 8 ou 9
            char primeiroDigito = nif[0];
            return primeiroDigito == '1' || primeiroDigito == '2' ||
                   primeiroDigito == '5' || primeiroDigito == '6' ||
                   primeiroDigito == '8' || primeiroDigito == '9';
        }

        // Validar Morada
        private bool ValidarMorada(string morada)
        {
            if (string.IsNullOrWhiteSpace(morada))
                return false;

            // Morada deve ter pelo menos 10 caracteres
            return morada.Length >= 10;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string cartao = textBox1.Text.Trim();
            string nif = textBox2.Text.Trim();
            string morada = textBox3.Text.Trim();

            // Validar Cartão de Cidadão
            if (string.IsNullOrWhiteSpace(cartao))
            {
                MessageBox.Show("Cartão de Cidadão ou Passaporte é obrigatório.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }

            bool cartaoValido = ValidarCartaoCidadao(cartao);
            bool passaporteValido = ValidarPassaporte(cartao);

            if (!cartaoValido && !passaporteValido)
            {
                MessageBox.Show(
                    "Cartão de Cidadão ou Passaporte inválido.\n\n" +
                    "Cartão de Cidadão: 12345678 AB\n" +
                    "Passaporte: AB123456",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }

            // Validar NIF
            if (!ValidarNIF(nif))
            {
                MessageBox.Show(
                    "NIF inválido.\n\n" +
                    "O NIF deve ter 9 dígitos e começar com 1, 2, 5, 6, 8 ou 9.\n" +
                    "Exemplo: 123456789",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Focus();
                return;
            }

            // Validar Morada
            if (!ValidarMorada(morada))
            {
                MessageBox.Show(
                    "Morada inválida.\n\n" +
                    "A morada deve ter pelo menos 10 caracteres.\n" +
                    "Exemplo: Rua da Paz, nº 10, 1000-001 Lisboa",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Focus();
                return;
            }

            // Se chegou aqui, tudo está válido
            this.Hide();
            using (var Loading = new Loading())
            {
                Loading.ShowDialog();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void documentos_Load(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Converter para maiúsculas automaticamente
            if (textBox1.Text.Length > 0)
            {
                int cursorPos = textBox1.SelectionStart;
                textBox1.Text = textBox1.Text.ToUpper();
                textBox1.SelectionStart = cursorPos;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Permitir apenas números
            string text = textBox2.Text;
            string cleaned = Regex.Replace(text, @"[^\d]", "");

            if (cleaned != text)
            {
                textBox2.Text = cleaned;
                textBox2.SelectionStart = cleaned.Length;
            }

            // Limitar a 9 dígitos
            if (textBox2.Text.Length > 9)
            {
                textBox2.Text = textBox2.Text.Substring(0, 9);
                textBox2.SelectionStart = 9;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }
    }
}