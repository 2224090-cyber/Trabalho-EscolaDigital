using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Horazon_Bank__projetoFinal.Conta;

namespace Horazon_Bank__projetoFinal
{
    public partial class perfil : Form
    {
        public perfil()
        {
            InitializeComponent();
        }

        private void VisibleChanged(object sender, EventArgs e)
        {
            AtualizarPerfil();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            AtualizarPerfil();
        }

        private void AtualizarPerfil()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(AtualizarPerfil));
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("PERFIL");
            sb.AppendLine($"Nome: {Conta.Nome} {Conta.Apelido}");
            sb.AppendLine($"Email: {Conta.Email}");
            sb.AppendLine($"ID: {Conta.Id}");
            sb.AppendLine($"Data de Nascimento: {Conta.DataFormatada}");
            sb.AppendLine($"Idade: {Conta.Idade} anos");

            sb.AppendLine("\nDOCUMENTOS");
            sb.AppendLine($"IDENTIFICAÇÃO: {Conta.CartaoCidadaoPassaporte}");
            sb.AppendLine($"NIF: {Conta.NIF}");
            sb.AppendLine($"Morada: {Conta.Morada}");

            sb.AppendLine("\nSALDO");
            sb.AppendLine($"Dinheiro em conta: {Conta.Saldo:C}");
            sb.AppendLine($"Dinheiro na poupança: {Conta.Poupanca:C}");

            label7.Text = sb.ToString();
        }

        private void perfil_Load(object sender, EventArgs e)
        {
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Form frm in Application.OpenForms.Cast<Form>().ToList())
            {
                frm.Hide();
            }

            this.Hide();
            using (var Form1 = new Form1())
            {
                Form1.ShowDialog();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Verifica se existe um empréstimo ativo com saldo devedor
            if (Conta.EmprestimoAtivo || Conta.SaldoDevedor > 0)
            {
                MessageBox.Show(
                    "Não é possível apagar a conta enquanto existir um empréstimo ativo.\n" +
                    "Por favor, quite o empréstimo antes de eliminar a conta.",
                    "Empréstimo Ativo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult resultado = MessageBox.Show(
                "Tem a certeza que deseja apagar a conta?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (resultado == DialogResult.Yes)
            {
                // Dados pessoais
                Conta.Nome = "";
                Conta.Apelido = "";
                Conta.Email = "";
                Conta.Dia = 0;
                Conta.Mes = 0;
                Conta.Ano = 0;
                Conta.Id = "";

                // Documentos
                Conta.CartaoCidadaoPassaporte = "";
                Conta.NIF = "";
                Conta.Morada = "";

                // Saldo e poupança
                Conta.Saldo = 0;
                Conta.Poupanca = 0;

                // Empréstimo
                Conta.SaldoDevedor = 0;
                Conta.ParcelaMensal = 0;
                Conta.EmprestimoAtivo = false;
                Conta.EmprestimoAprovado = false;
                Conta.LimparHistorico();

                // Histórico
                Conta.Historico = new List<string>();

                MessageBox.Show("Conta apagada com sucesso!");

                foreach (Form frm in Application.OpenForms.Cast<Form>().ToList())
                {
                    frm.Hide();
                }


                this.Hide();
                using (var Form1 = new Form1())
                {
                    Form1.ShowDialog();
                }

            }
        }

    }
}