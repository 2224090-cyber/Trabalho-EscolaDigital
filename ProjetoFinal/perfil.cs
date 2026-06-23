using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static Horazon_Bank__projetoFinal.Conta;

namespace Horazon_Bank__projetoFinal
{
    public partial class perfil : Form
    {
        public perfil()
        {
            InitializeComponent();
            Conta.ValoresAlterados += AtualizarPerfil;
        }

        private void VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                SincronizarBancoEDados();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            SincronizarBancoEDados();
        }

        private void SincronizarBancoEDados()
        {
            if (!string.IsNullOrEmpty(Conta.Email))
            {
                Conta.CarregarDadosDoSQL(Conta.Email);
            }
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

        private void perfil_Load(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }

        // ==========================================================
        // --- BUTTON 1: FAZER LOG OUT (VOLTAR PARA O LOGIN) ---
        // ==========================================================
        private void button1_Click(object sender, EventArgs e)
        {
            // ✅ CORREÇÃO: Fecha TODOS os formulários abertos na aplicação (incluindo o menu_principal)
            List<Form> formulariosAbertos = Application.OpenForms.Cast<Form>().ToList();
            foreach (Form frm in formulariosAbertos)
            {
                if (frm.Name != "Form1") // Fecha tudo o que NÃO for o ecrã de Login
                {
                    frm.Hide(); // Esconde primeiro para não dar piscar de ecrã
                    frm.Close();
                }
            }

            // Limpeza básica de segurança ao sair
            Conta.Email = "";

            // ✅ Abre o Login de forma limpa e isolada
            Form1 login = new Form1();
            login.Show();
        }

        // ==========================================================
        // --- BUTTON 2: ELIMINAR CONTA (DELETE NO SQL) ---
        // ==========================================================
        private void button2_Click(object sender, EventArgs e)
        {
            // 1. Verifica se existe um empréstimo ativo
            if (Conta.EmprestimoAtivo || Conta.SaldoDevedor > 0)
            {
                MessageBox.Show(
                    "Não é possível apagar a conta enquanto existir um empréstimo ativo.\n" +
                    "Por favor, liquide o empréstimo antes de eliminar a conta.",
                    "Empréstimo Ativo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 2. Pede confirmação
            DialogResult resultado = MessageBox.Show(
                "Tem a certeza que deseja apagar permanentemente a sua conta?",
                "Confirmar Eliminação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (resultado == DialogResult.Yes)
            {
                string emailUsuario = Conta.Email;

                using (SqlConnection conexao = Database.GetConnection())
                {
                    try
                    {
                        conexao.Open();
                        string queryDeletar = "DELETE FROM Utilizadores WHERE Email = @Email";

                        using (SqlCommand cmdDeletar = new SqlCommand(queryDeletar, conexao))
                        {
                            cmdDeletar.Parameters.AddWithValue("@Email", emailUsuario);
                            cmdDeletar.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao eliminar registo da Base de Dados: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // 3. LIMPEZA DOS DADOS DA SESSÃO LOCAL (MEMÓRIA RAM)
                Conta.Nome = "";
                Conta.Apelido = "";
                Conta.Email = "";
                Conta.Dia = 0;
                Conta.Mes = 0;
                Conta.Ano = 0;
                Conta.Id = "";
                Conta.CartaoCidadaoPassaporte = "";
                Conta.NIF = "";
                Conta.Morada = "";
                Conta.Saldo = 0;
                Conta.Poupanca = 0;
                Conta.SaldoDevedor = 0;
                Conta.ParcelaMensal = 0;
                Conta.EmprestimoAtivo = false;
                Conta.EmprestimoAprovado = false;
                Conta.Historico = new List<string>();

                MessageBox.Show("Conta eliminada com sucesso da base de dados do Horizon Bank!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // ✅ CORREÇÃO: Fecha o menu_principal e outros ecrãs antes de voltar ao Login
                List<Form> formulariosParaFechar = Application.OpenForms.Cast<Form>().ToList();
                foreach (Form frm in formulariosParaFechar)
                {
                    if (frm.Name != "Form1")
                    {
                        frm.Hide();
                        frm.Close();
                    }
                }

                // Abre o ecrã de Login limpo
                Form1 login = new Form1();
                login.Show();
            }
        }
    }
}