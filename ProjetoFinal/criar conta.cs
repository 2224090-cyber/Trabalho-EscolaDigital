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
            guna2ComboBox4.Items.Add("Homem");
            guna2ComboBox4.Items.Add("Mulher");
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

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

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

            // Se chegou aqui, todos os dados são válidos
            this.Hide();
            using (var verificacão_De_Cont = new verificacão_de_conta())
            {
                verificacão_De_Cont.ShowDialog();
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
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}