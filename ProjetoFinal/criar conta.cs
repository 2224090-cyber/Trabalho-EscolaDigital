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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;


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

        // Validar se o texto tem apenas letras (sem números)
        private bool ValidarNomeApelido(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            // Permitir apenas letras e espaços
            return Regex.IsMatch(texto, @"^[a-záéíóúàâãôõçñA-ZÁÉÍÓÚÀÂÃÔÕÇÑ\s]+$");
        }

        // Validar email
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

        // Validar se a data é válida para o mês selecionado
        private bool ValidarData(int dia, int mes, int ano)
        {
            if (dia < 1 || mes < 1 || mes > 12 || ano < 1915)
                return false;

            int[] diasPorMes = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            // Verificar ano bissexto
            if (mes == 2 && ((ano % 4 == 0 && ano % 100 != 0) || (ano % 400 == 0)))
                return dia <= 29;

            return dia <= diasPorMes[mes - 1];
        }

        // Calcular idade a partir da data de nascimento
        private int CalcularIdade(int dia, int mes, int ano)
        {
            DateTime dataNascimento = new DateTime(ano, mes, dia);
            DateTime hoje = DateTime.Now;

            int idade = hoje.Year - dataNascimento.Year;

            if (dataNascimento.Date > hoje.AddYears(-idade))
                idade--;

            return idade;
        }

        // ===================== CAPITALIZAÇÃO DE TEXTO =====================

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

        // ===================== CLIQUES (vazios / utilitários) =====================

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void label6_Click(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
        }

        private void label_ForcaSenha_Click(object sender, EventArgs e)
        {
        }

        private void label8_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // ===================== CRIAR CONTA =====================

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            // Validar Nome
            if (!ValidarNomeApelido(textBox1.Text))
            {
                MessageBox.Show("Nome inválido. Use apenas letras (sem números).", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }

            // Validar Apelido
            if (!ValidarNomeApelido(textBox2.Text))
            {
                MessageBox.Show("Apelido inválido. Use apenas letras (sem números).", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Focus();
                return;
            }

            // Validar Dia
            if (guna2ComboBox1.SelectedIndex == 0)
            {
                MessageBox.Show("Selecione um dia válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                guna2ComboBox1.Focus();
                return;
            }

            // Validar Mês
            if (guna2ComboBox2.SelectedIndex == 0)
            {
                MessageBox.Show("Selecione um mês válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                guna2ComboBox2.Focus();
                return;
            }

            // Validar Ano
            if (guna2ComboBox3.SelectedIndex == 0)
            {
                MessageBox.Show("Selecione um ano válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                guna2ComboBox3.Focus();
                return;
            }

            // Validar Gênero
            if (guna2ComboBox4.SelectedIndex == 0)
            {
                MessageBox.Show("Selecione um gênero.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                guna2ComboBox4.Focus();
                return;
            }

            // Obter valores
            int dia = int.Parse(guna2ComboBox1.SelectedItem.ToString());
            int mes = guna2ComboBox2.SelectedIndex; // 1-12
            int ano = int.Parse(guna2ComboBox3.SelectedItem.ToString());

            // Validar se a data é válida para o mês
            if (!ValidarData(dia, mes, ano))
            {
                MessageBox.Show($"Data inválida. O mês {guna2ComboBox2.SelectedItem} não tem {dia} dias.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                guna2ComboBox1.Focus();
                return;
            }

            // Validar idade mínima (18 anos)
            int idade = CalcularIdade(dia, mes, ano);

            if (idade < 18)
            {
                MessageBox.Show(
                    "É necessário ter no mínimo 18 anos para criar uma conta no Horizon Bank.\nO programa será encerrado.",
                    "Idade Insuficiente",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Application.Exit();
                return;
            }

            // Validar Email
            if (!ValidarEmail(guna2TextBox4.Text))
            {
                MessageBox.Show("Email inválido. Insira um email válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                guna2TextBox4.Focus();
                return;
            }

            // Validar Senha
            if (string.IsNullOrWhiteSpace(guna2TextBox5.Text))
            {
                MessageBox.Show("A senha é obrigatória.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                guna2TextBox5.Focus();
                return;
            }

            if (guna2TextBox5.Text.Length < 6)
            {
                MessageBox.Show("A senha deve ter no mínimo 6 caracteres.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                guna2TextBox5.Focus();
                return;
            }


        

            Conta.CodigoVerificacao = Conta.GerarCodigoVerificacao();

            email emailBanco = new email(
                "smtp.gmail.com",
                587,
                "Horizonbank.f1@gmail.com",      // <-- email do banco
                "liog nhuo xddf jpwk"            // <-- senha de app do Gmail
            );

            bool enviado = emailBanco.EnviarCodigoVerificacao(guna2TextBox4.Text, Conta.CodigoVerificacao);

            if (!enviado)
            {
                MessageBox.Show("Não foi possível enviar o código de verificação. Verifique o email e tente novamente.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.Hide();
            using (var verificacão_De_Cont = new verificacao_de_conta())
            {
                verificacão_De_Cont.ShowDialog();
            }

            // Só guarda os dados da conta DEPOIS da verificação ser bem-sucedida
            // (ver nota abaixo sobre mover isto para dentro do verificacao_de_conta)
            Conta.Nome = textBox1.Text;
            Conta.Apelido = textBox2.Text;
            Conta.Dia = int.Parse(guna2ComboBox1.SelectedItem.ToString());
            Conta.Mes = guna2ComboBox2.SelectedIndex;
            Conta.Ano = int.Parse(guna2ComboBox3.SelectedItem.ToString());
            Conta.Email = guna2TextBox4.Text;
            Conta.Senha = guna2TextBox5.Text;
            Conta.Id = Conta.GerarId();

            this.Hide();
            using (var menuPrincipal = new menu_principal())
            {
                menuPrincipal.ShowDialog();
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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
        }

        private void criar_conta_Load(object sender, EventArgs e)
        {
            PreencherComboBoxes();
            guna2TextBox5.PasswordChar = '*';
            button1.Text = "Mostrar";
        }

        // ===================== NOME (textBox1) =====================

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int posicaoCursor = textBox1.SelectionStart;
            string texto = textBox1.Text;

            // Se o utilizador digitou um espaço, remove-o e salta para o apelido
            if (texto.EndsWith(" "))
            {
                textBox1.Text = texto.TrimEnd();
                textBox2.Focus();
                textBox2.SelectionStart = textBox2.Text.Length;
                return;
            }

            if (string.IsNullOrEmpty(texto))
                return;

            string textoCapitalizado = CapitalizarTexto(texto);

            if (textoCapitalizado != texto)
            {
                textBox1.Text = textoCapitalizado;
                textBox1.SelectionStart = posicaoCursor;
            }
        }

        // ===================== APELIDO (textBox2) =====================

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int posicaoCursor = textBox2.SelectionStart;
            string texto = textBox2.Text;

            if (string.IsNullOrEmpty(texto))
                return;

            string textoCapitalizado = CapitalizarTexto(texto);

            if (textoCapitalizado != texto)
            {
                textBox2.Text = textoCapitalizado;
                textBox2.SelectionStart = posicaoCursor;
            }
        }

        // ===================== MOSTRAR/OCULTAR SENHA =====================

        private void button1_Click(object sender, EventArgs e)
        {
            if (guna2TextBox5.PasswordChar == '*')
            {
                guna2TextBox5.PasswordChar = '\0'; // mostra a senha
                button1.Text = "";
            }
            else
            {
                guna2TextBox5.PasswordChar = '*'; // volta a ocultar
                button1.Text = "";
            }
        }

        private void guna2ComboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }
    }
}