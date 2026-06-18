using Horazon_Bank__projetoFinal;
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
        public Emprestimos()
        {
            InitializeComponent();
            Conta.ValoresAlterados += AtualizarValores;

        }

        public static decimal SaldoDevedor { get; set; } = 0;
        public static decimal ParcelaMensal { get; set; } = 0;
        public static bool EmprestimoAtivo { get; set; } = false;
        public static bool EmprestimoAprovado { get; set; } = false;

        private void AtualizarEmprestimo()
        {
            textBox3.Text = Conta.Poupanca.ToString("F2");

            if (Conta.EmprestimoAtivo)
            {
                label6.Text = "Empréstimo Ativo";
                label7.Text =
                    $"Total a pagar: {Conta.SaldoDevedor:C}\n" +
                    $"Parcela mensal: {Conta.ParcelaMensal:C}";

                // Bloquear campos
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox4.Enabled = false;
                button1.Enabled = false;
            }
            else
            {
                label6.Text = "";
                label7.Text = "";
                label8.Text = "";

                // Desbloquear campos
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox4.Enabled = true;
                button1.Enabled = true;
            }
        }

        private void FormEmprestimo_Load(object sender, EventArgs e)
        {
            AtualizarEmprestimo();
        }

        private void AtualizarValores()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(AtualizarValores));
                return;
            }

            textBox3.Text = Conta.Poupanca.ToString("F2");
            AtualizarEmprestimo();
        }

        private void Emprestimos_VisibleChanged(object sender, EventArgs e)
        {
            AtualizarEmprestimo();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            AtualizarEmprestimo();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Conta.ValoresAlterados -= AtualizarValores;
            base.OnFormClosed(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Conta.EmprestimoAtivo)
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
            decimal poupanca = Conta.Poupanca;
            decimal valorEmprestimo;
            int prazo;

            if (!decimal.TryParse(textBox1.Text, out salario) ||
                !decimal.TryParse(textBox2.Text, out valorEmprestimo) ||
                !int.TryParse(textBox4.Text, out prazo))
            {
                MessageBox.Show("Digite apenas números válidos.");
                return;
            }

            if (salario <= 0 || valorEmprestimo <= 0 || prazo <= 0)
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
                juros = 0.15m;

            decimal valorTotal = valorEmprestimo + (valorEmprestimo * juros);

            Conta.ParcelaMensal = valorTotal / prazo;
            Conta.SaldoDevedor = valorTotal;

            label7.Text =
                $"Total a pagar: {valorTotal:C}\n" +
                $"Parcela mensal: {Conta.ParcelaMensal:C}";

            if (Conta.ParcelaMensal <= salario * 0.30m)
            {
                Conta.EmprestimoAprovado = true;
                label8.Text = "Empréstimo APROVADO";
            }
            else
            {
                Conta.EmprestimoAprovado = false;
                label8.Text = "Empréstimo NÃO APROVADO";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Conta.SaldoDevedor <= 0)
            {
                MessageBox.Show("Nenhum empréstimo válido foi calculado.");
                return;
            }

            if (!Conta.EmprestimoAprovado)
            {
                MessageBox.Show("Empréstimo não aprovado.");
                return;
            }

            if (Conta.EmprestimoAtivo)
            {
                MessageBox.Show("Já existe um empréstimo ativo.");
                return;
            }

            decimal valorEmprestimo = decimal.Parse(textBox2.Text);

            Conta.Saldo += valorEmprestimo;
            Conta.AdicionarHistorico($"Emprestimo: +{valorEmprestimo:C}");

            Conta.EmprestimoAtivo = true;

            label6.Text = "Empréstimo Ativo";

            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox4.Enabled = false;
            button1.Enabled = false;

            MessageBox.Show("Empréstimo aceito com sucesso.");
            AtualizarEmprestimo();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (!Conta.EmprestimoAtivo)
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

            if (pagamento > Conta.Saldo)
            {
                MessageBox.Show("Saldo insuficiente na conta.");
                return;
            }

            // ✅ NOVO: Se pagar mais que deve, devolve o excedente
            decimal pagamentoReal = pagamento;
            decimal excedente = 0;

            if (pagamento > Conta.SaldoDevedor)
            {
                excedente = pagamento - Conta.SaldoDevedor;
                pagamentoReal = Conta.SaldoDevedor;

                MessageBox.Show($"Você pagou {excedente:C} a mais. Esse valor será devolvido à sua conta.");
            }

            Conta.Saldo -= pagamentoReal;
            Conta.Saldo += excedente; // Devolve o excedente
            Conta.SaldoDevedor -= pagamentoReal;
            Conta.AdicionarHistorico($"Pagamento do emprestimo: -{pagamentoReal:C}");

            if (Conta.SaldoDevedor <= 0)
            {
                Conta.SaldoDevedor = 0;
                Conta.ParcelaMensal = 0;

                Conta.EmprestimoAtivo = false;
                Conta.EmprestimoAprovado = false;

                Conta.AdicionarHistorico("Empréstimo quitado");

                textBox1.Clear();
                textBox2.Clear();
                textBox4.Clear();
                textBox5.Clear();

                textBox3.Text = Conta.Poupanca.ToString("F2");

                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox4.Enabled = true;
                button1.Enabled = true;

                label7.Text = "";
                label8.Text = "";
                label6.Text = "";

                MessageBox.Show("Empréstimo quitado com sucesso!");
            }
            else
            {
                label7.Text =
                    $"Total a pagar: {Conta.SaldoDevedor:C}\n" +
                    $"Parcela mensal: {Conta.ParcelaMensal:C}";

                textBox5.Clear();
                MessageBox.Show("Pagamento realizado com sucesso.");
            }

            AtualizarEmprestimo();
        }

        private void Emprestimos_Load(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }

        private void label5_Click(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }
}
