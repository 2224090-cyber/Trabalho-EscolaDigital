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
    public partial class Poupanca : Form
    {

      
            public Poupanca()
            {
                InitializeComponent();

                Conta.ValoresAlterados += AtualizarValores;
            }

        

        private void Poupanca_VisibleChanged(object sender, EventArgs e)
        {
            AtualizarValores();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            AtualizarValores();
        }

        private void AtualizarValores()
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(AtualizarValores));
                    return;
                }

                label2.Text = $"Poupança: {Conta.Poupanca:C}";
            }

            private void Poupanca_Load(object sender, EventArgs e)
            {
                AtualizarValores();
            }

            private void button1_Click(object sender, EventArgs e)
            {


            decimal guardar = 0;
            decimal retirar = 0;

            bool querGuardar = !string.IsNullOrWhiteSpace(textBox1.Text);
            bool querRetirar = !string.IsNullOrWhiteSpace(textBox2.Text);

            if (!querGuardar && !querRetirar)
            {
                MessageBox.Show("Digite um valor.");
                return;
            }

            if (querGuardar && querRetirar)
            {
                MessageBox.Show("Preencha apenas um campo de cada vez.");
                return;
            }

            // GUARDAR DINHEIRO
            if (querGuardar)
            {
                if (!decimal.TryParse(textBox1.Text, out guardar))
                {
                    MessageBox.Show("Valor inválido.");
                    return;
                }

                if (guardar <= 0)
                {
                    MessageBox.Show("O valor deve ser maior que zero.");
                    return;
                }

                if (guardar > Conta.Saldo)
                {
                    MessageBox.Show("Saldo insuficiente.");
                    return;
                }

                Conta.Saldo -= guardar;
                Conta.Poupanca += guardar;

                Conta.AdicionarHistorico($"Poupança: -{guardar:C})");

                MessageBox.Show("Valor guardado na poupança.");
            }

            // RETIRAR DINHEIRO
            if (querRetirar)
            {
                if (!decimal.TryParse(textBox2.Text, out retirar))
                {
                    MessageBox.Show("Valor inválido.");
                    return;
                }

                if (retirar <= 0)
                {
                    MessageBox.Show("O valor deve ser maior que zero.");
                    return;
                }

                if (retirar > Conta.Poupanca)
                {
                    MessageBox.Show("Saldo insuficiente na poupança.");
                    return;
                }

                Conta.Poupanca -= retirar;
                Conta.Saldo += retirar;

                Conta.AdicionarHistorico($"Poupança: +{retirar:C}");

                MessageBox.Show("Valor retirado da poupança.");
            }

            AtualizarValores();

            textBox1.Clear();
            textBox2.Clear();

        }

        protected override void OnFormClosed(FormClosedEventArgs e)
            {
                Conta.ValoresAlterados -= AtualizarValores;
                base.OnFormClosed(e);
            }

        private void label2_Click(object sender, EventArgs e)
        {
            label2.Text = $"Poupança: {Conta.Poupanca:C}";
        }
    }
    }
