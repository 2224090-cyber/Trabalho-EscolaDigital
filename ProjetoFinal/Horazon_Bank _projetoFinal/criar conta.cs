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
using System.Data.SqlClient;

namespace Horazon_Bank__projetoFinal
{
    public partial class criar_conta : Form
    {
        public criar_conta()
        {
            InitializeComponent();
        }

        private void PreencherComboBoxes()
        {
            // Preencher dias (1-31)
            guna2ComboBox1.Items.Clear();
            guna2ComboBox1.Items.Add("Selecione um dia");
            for (int i = 1; i <= 31; i++)
            {
                guna2ComboBox1.Items.Add(i.ToString("D2"));
            }
            guna2ComboBox1.SelectedIndex = 0;

            // Preencher meses
            guna2ComboBox2.Items.Clear();
            guna2ComboBox2.Items.Add("Selecione um mês");
            string[] meses = { "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
                               "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };
            foreach (string mes in meses)
            {
                guna2ComboBox2.Items.Add(mes);
            }
            guna2ComboBox2.SelectedIndex = 0;

            // Preencher anos (até 1915)
            guna2ComboBox3.Items.Clear();
            guna2ComboBox3.Items.Add("Selecione um ano");
            int anoAtual = DateTime.Now.Year;
            for (int i = anoAtual; i >= 1915; i--)
            {
                guna2ComboBox3.Items.Add(i.ToString());
            }
            guna2ComboBox3.SelectedIndex = 0;

            // Preencher gênero
            guna2ComboBox4.Items.Clear();
            guna2ComboBox4.Items.Add("Selecione o gênero");
            guna2ComboBox4.Items.Add("Masculino");
            guna2ComboBox4.Items.Add("Feminino");
            guna2ComboBox4.Items.Add("Outro");
            guna2ComboBox4.SelectedIndex = 0;
        }

        private bool ValidarNomeApelido(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            return Regex.IsMatch(texto, @"^[a-záéíóúàâãôõçñA-ZÁÉÍÓÚÀÂÃÔÕÇÑ\s]+$");
        }

        private bool ValidarEmail(string email)
        {
            try
            {
                var endereco = new System.Net.Mail.MailAddress(email);
                return endereco.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidarData(int dia, int mes, int ano)
        {
            if (dia < 1 || mes < 1 || mes > 12 || ano < 1915)
                return false;

            int[] diasPorMes = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            if (mes == 2 && ((ano % 4 == 0 && ano % 100 != 0) || (ano % 400 == 0)))
                return dia <= 29;

            return dia <= diasPorMes[mes - 1];
        }

        private int CalcularIdade(int dia, int mes, int ano)
        {
            DateTime dataNascimento = new DateTime(ano, mes, dia);
            DateTime hoje = DateTime.Now;

            int idade = hoje.Year - dataNascimento.Year;

            if (dataNascimento.Date > hoje.AddYears(-idade))
                idade--;

            return idade;
        }

        private string CapitalizarTexto(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            string[] palavras = texto.Split(' ');

            for (int i = 0; i < palavras.Length; i++)
            {
                if (palavras[i].Length > 0)
                {
                    palavras[i] = char.ToUpper(palavras[i][0]) +
                                  (palavras[i].Length > 1 ? palavras[i].Substring(1) : "");
                }
            }

            return string.Join(" ", palavras);
        }

        // =========================================================================
        // --- BOTÃO: AVANÇAR / SOLICITAR ENVIAR CÓDIGO ---
        // =========================================================================
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            // Validações de Interface Básicas
            if (!ValidarNomeApelido(textBox1.Text))
            {
                MessageBox.Show("Nome inválido. Use apenas letras.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }

            if (!ValidarNomeApelido(textBox2.Text))
            {
                MessageBox.Show("Apelido inválido. Use apenas letras.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Focus();
                return;
            }

            if (guna2ComboBox1.SelectedIndex == 0 || guna2ComboBox2.SelectedIndex == 0 || guna2ComboBox3.SelectedIndex == 0 || guna2ComboBox4.SelectedIndex == 0)
            {
                MessageBox.Show("Preencha todos os campos de data e gênero corretamente.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int dia = int.Parse(guna2ComboBox1.SelectedItem.ToString());
            int mes = guna2ComboBox2.SelectedIndex;
            int ano = int.Parse(guna2ComboBox3.SelectedItem.ToString());

            if (!ValidarData(dia, mes, ano))
            {
                MessageBox.Show($"Data inválida. O mês {guna2ComboBox2.SelectedItem} não possui {dia} dias.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (CalcularIdade(dia, mes, ano) < 18)
            {
                MessageBox.Show("É necessário ter no mínimo 18 anos para criar uma conta.", "Idade Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!ValidarEmail(guna2TextBox4.Text))
            {
                MessageBox.Show("Email inválido. Insira um email estruturado corretamente.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                guna2TextBox4.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(guna2TextBox5.Text) || guna2TextBox5.Text.Length < 6)
            {
                MessageBox.Show("A senha deve possuir no mínimo 6 caracteres.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                guna2TextBox5.Focus();
                return;
            }

            // --- SEGURANÇA: VERIFICAR SE O EMAIL JÁ EXISTE NO SQL ---
            using (SqlConnection conexao = Database.GetConnection())
            {
                try
                {
                    conexao.Open();
                    string queryVerificar = "SELECT COUNT(1) FROM Utilizadores WHERE Email = @Email";
                    using (SqlCommand cmd = new SqlCommand(queryVerificar, conexao))
                    {
                        cmd.Parameters.AddWithValue("@Email", guna2TextBox4.Text.Trim());
                        int existe = Convert.ToInt32(cmd.ExecuteScalar());

                        if (existe > 0)
                        {
                            MessageBox.Show("Este endereço de email já se encontra registado no sistema.", "Email Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            guna2TextBox4.Focus();
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao validar credenciais na base de dados: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // --- PROCESSO DE ENVIO DE EMAIL ---
            Conta.CodigoVerificacao = Conta.GerarCodigoVerificacao();

            email emailBanco = new email(
                "smtp.gmail.com",
                587,
                "Horizonbank.f1@gmail.com",
                "liog nhuo xddf jpwk"
            );

            bool enviado = emailBanco.EnviarCodigoVerificacao(guna2TextBox4.Text, Conta.CodigoVerificacao);

            if (!enviado)
            {
                MessageBox.Show("Não foi possível enviar o código de verificação. Verifique a sua conexão.", "Erro SMTP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Aloca temporariamente os dados na RAM (Classe Estática)
            Conta.Nome = textBox1.Text;
            Conta.Apelido = textBox2.Text;
            Conta.Dia = dia;
            Conta.Mes = mes;
            Conta.Ano = ano;
            Conta.Email = guna2TextBox4.Text.Trim();
            Conta.Senha = guna2TextBox5.Text;
            Conta.Id = Conta.GerarId(); // Gera um número de conta/Id único aleatório

            // Abre a tela de verificação de token
            this.Hide();
            using (var verificacaoForm = new verificacao_de_conta())
            {
                verificacaoForm.ShowDialog();
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var Form1 = new Form1())
            {
                Form1.ShowDialog();
            }
        }

        private void criar_conta_Load(object sender, EventArgs e)
        {
            PreencherComboBoxes();
            guna2TextBox5.PasswordChar = '*';
            button1.Text = "Mostrar";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int posicaoCursor = textBox1.SelectionStart;
            string texto = textBox1.Text;

            if (texto.EndsWith(" "))
            {
                textBox1.Text = texto.TrimEnd();
                textBox2.Focus();
                textBox2.SelectionStart = textBox2.Text.Length;
                return;
            }

            if (string.IsNullOrEmpty(texto)) return;

            string textoCapitalizado = CapitalizarTexto(texto);
            if (textoCapitalizado != texto)
            {
                textBox1.Text = textoCapitalizado;
                textBox1.SelectionStart = posicaoCursor;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int posicaoCursor = textBox2.SelectionStart;
            string texto = textBox2.Text;

            if (string.IsNullOrEmpty(texto)) return;

            string textoCapitalizado = CapitalizarTexto(texto);
            if (textoCapitalizado != texto)
            {
                textBox2.Text = textoCapitalizado;
                textBox2.SelectionStart = posicaoCursor;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (guna2TextBox5.PasswordChar == '*')
            {
                guna2TextBox5.PasswordChar = '\0';
            }
            else
            {
                guna2TextBox5.PasswordChar = '*';
            }
        }

        private void button3_Click(object sender, EventArgs e) => Application.Exit();
        private void label3_Click(object sender, EventArgs e) { }
        private void label6_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void pictureBox2_Click(object sender, EventArgs e) { }
        private void label_ForcaSenha_Click(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { }
        private void guna2ComboBox4_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label3_Click_1(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
    }
}