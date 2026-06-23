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
    public partial class Deposito_Saque : Form
    {
        public Deposito_Saque()
        {
            InitializeComponent();
        }

      

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            decimal valor;

            if (!decimal.TryParse(textBox1.Text, out valor))
            {
                MessageBox.Show("Digite um valor válido.");
                return;
            }

            if (valor <= 0)
            {
                MessageBox.Show("O valor deve ser maior que zero.");
                return;
            }

            Conta.Saldo += valor;
            Conta.AdicionarHistorico($"Depósito: +{valor:C}");

            MessageBox.Show("Depósito realizado com sucesso.");

            textBox1.Clear();


        }

        private void button2_Click(object sender, EventArgs e)
        {
            decimal valor;

            if (!decimal.TryParse(textBox2.Text, out valor))
            {
                MessageBox.Show("Digite um valor válido.");
                return;
            }

            if (valor <= 0)
            {
                MessageBox.Show("O valor deve ser maior que zero.");
                return;
            }

            if (valor > Conta.Saldo)
            {
                MessageBox.Show("Saldo insuficiente.");
                return;
            }

            Conta.Saldo -= valor;
            Conta.AdicionarHistorico($"Saque: -{valor:C}");

            MessageBox.Show("Saque realizado com sucesso.");

            textBox2.Clear();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
