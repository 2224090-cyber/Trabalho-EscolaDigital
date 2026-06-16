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
    public partial class Emprestimos : Form
    {
        decimal saldoDevedor = 0;
        decimal parcelaMensal = 0;

        bool emprestimoAprovado = false;
        bool emprestimoAtivo = false;

        private void ApenasNumerosVirgula_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !char.IsDigit(e.KeyChar) &&
                e.KeyChar != ',')
            {
                e.Handled = true;
            }

            TextBox txt = (TextBox)sender;

            if (e.KeyChar == ',' && txt.Text.Contains(","))
            {
                e.Handled = true;
            }
        }



        public Emprestimos()
        {
            InitializeComponent();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (emprestimoAtivo)
            {
                MessageBox.Show("Já existe um empréstimo ativo. Quite-o antes de solicitar outro.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Preencha todos os campos obrigatórios.");
                return;
            }

            decimal salario;
            decimal poupanca = 0;
            decimal valorEmprestimo;
            int prazo;

            if (!decimal.TryParse(textBox1.Text, out salario) ||
                !decimal.TryParse(textBox2.Text, out valorEmprestimo) ||
                !int.TryParse(textBox4.Text, out prazo))
            {
                MessageBox.Show("Digite apenas números válidos.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(textBox3.Text))
            {
                decimal.TryParse(textBox3.Text, out poupanca);
            }

            if (salario <= 0 || valorEmprestimo <= 0 || prazo <= 0 || poupanca < 0)
            {
                MessageBox.Show("Valores inválidos.");
                return;
            }

            decimal juros;

            if (poupanca >= 10000)
                juros = 0.05m;
            else if (poupanca >= 5000)
                juros = 0.10m;
            else
                juros = 0.25m;

            decimal valorTotal = valorEmprestimo + (valorEmprestimo * juros);

            parcelaMensal = valorTotal / prazo;

            saldoDevedor = valorTotal;

            label7.Text =
                $"Total a pagar: {valorTotal:C}\n" +
                $"Parcela mensal: {parcelaMensal:C}";

            if (parcelaMensal <= salario * 0.30m)
            {
                emprestimoAprovado = true;
                label8.Text = "Empréstimo APROVADO";
            }
            else
            {
                emprestimoAprovado = false;
                label8.Text = "Empréstimo NÃO APROVADO";
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (saldoDevedor <= 0)
            {
                MessageBox.Show("Nenhum empréstimo válido foi calculado.");
                return;
            }

            if (!emprestimoAprovado)
            {
                MessageBox.Show("Empréstimo não aprovado.");
                return;
            }

            if (emprestimoAtivo)
            {
                MessageBox.Show("Já existe um empréstimo ativo.");
                return;
            }

            emprestimoAtivo = true;

            label6.Text = "Empréstimo Ativo";

            textBox1.Enabled = false;
            textBox3.Enabled = false;
            textBox2.Enabled = false;
            textBox4.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (!emprestimoAtivo)
            {
                MessageBox.Show("Não existe empréstimo ativo.");
                return;
            }

            decimal pagamento;

            if (!decimal.TryParse(textBox5.Text, out pagamento))
            {
                MessageBox.Show("Digite um valor válido.");
                return;
            }

            if (pagamento <= 0)
            {
                MessageBox.Show("O pagamento deve ser maior que zero.");
                return;
            }

            saldoDevedor -= pagamento;

            if (saldoDevedor <= 0)
            {
                saldoDevedor = 0;
                parcelaMensal = 0;

                emprestimoAtivo = false;
                emprestimoAprovado = false;

                textBox1.Clear();
                textBox3.Clear();
                textBox2.Clear();
                textBox4.Clear();
                textBox5.Clear();

                textBox1.Enabled = true;
                textBox3.Enabled = true;
                textBox2.Enabled = true;
                textBox4.Enabled = true;
                textBox5.Enabled = true;

                label7.Text = "";
                label8.Text = "";
                label6.Text = "";

                MessageBox.Show("Empréstimo quitado com sucesso!");
            }
            else
            {
                label7.Text =
                    $"Total a pagar: {saldoDevedor:C}\n" +
                    $"Parcela mensal: {parcelaMensal:C}";

                textBox5.Clear();
            }

        }

        private void Emprestimos_Load(object sender, EventArgs e)
        {

        }
    }
}
