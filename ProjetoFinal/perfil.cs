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
using System.Data.SqlClient; 

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
        // --- BUTTON 1: FAZER LOG OUT (REUTILIZA LOGIN ORIGINAL) ---
        // ==========================================================
        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Limpeza de segurança dos dados na memória RAM ao sair
            Conta.Email = "";
            Conta.LimparHistorico();

            // 2. Procura se o Form1 original (oculto) já existe na memória
            Form formLoginOriginal = Application.OpenForms["Form1"];

            // 3. Se o Form1 original existir, limpa os campos e mostra-o
            if (formLoginOriginal != null)
            {
                var txtEmail = formLoginOriginal.Controls.Find("txtEmail", true).FirstOrDefault() as TextBox;
                var txtSenha = formLoginOriginal.Controls.Find("txtSenha", true).FirstOrDefault() as TextBox;

                if (txtEmail != null) txtEmail.Text = "";
                if (txtSenha != null) txtSenha.Text = "";

                formLoginOriginal.Show();
            }
            else
            {
                Form1 novoLogin = new Form1();
                novoLogin.Show();
            }

            // 4. Fecha todos os outros formulários abertos (Menu Principal, Perfil, etc.)
            List<Form> formulariosAbertos = Application.OpenForms.Cast<Form>().ToList();
            foreach (Form frm in formulariosAbertos)
            {
                if (frm.Name != "Form1")
                {
                    frm.Hide();
                    frm.Close();
                }
            }
        }

        // ==========================================================
        // --- BUTTON 2: ELIMINAR CONTA (REUTILIZA LOGIN ORIGINAL) --
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
                "Tem a certeza que deseja apagar permanentemente a sua conta?\n" +
                "Isto irá eliminar todo o seu histórico de transferências.",
                "Confirmar Eliminação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (resultado == DialogResult.Yes)
            {
                string emailUsuario = Conta.Email;
                string idUsuario = Conta.Id;

                using (SqlConnection conexao = Database.GetConnection())
                {
                    try
                    {
                        conexao.Open();

                        // A. Apagar primeiro o histórico de transações simples do utilizador
                        string queryHist = "DELETE FROM HistoricoTransacoes WHERE UsuarioId = @Id";
                        using (SqlCommand cmdHist = new SqlCommand(queryHist, conexao))
                        {
                            cmdHist.Parameters.AddWithValue("@Id", idUsuario);
                            cmdHist.ExecuteNonQuery();
                        }

                        // B. Apagar as transferências onde ele foi o Remetente ou o Destinatário
                        string queryTransf = "DELETE FROM Transferencias WHERE RemetenteId = @Id OR DestinatarioId = @Id";
                        using (SqlCommand cmdTransf = new SqlCommand(queryTransf, conexao))
                        {
                            cmdTransf.Parameters.AddWithValue("@Id", idUsuario);
                            cmdTransf.ExecuteNonQuery();
                        }

                        // C. Agora que os vínculos foram limpos, apagamos o utilizador principal
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

                // 4. Procura se o Form1 original (oculto) já existe na memória para evitar duplicados
                Form formLoginOriginal = Application.OpenForms["Form1"];

                if (formLoginOriginal != null)
                {
                    // Se o Login original existir, limpa as caixas de texto dele e exibe-o
                    var txtEmail = formLoginOriginal.Controls.Find("txtEmail", true).FirstOrDefault() as TextBox;
                    var txtSenha = formLoginOriginal.Controls.Find("txtSenha", true).FirstOrDefault() as TextBox;

                    if (txtEmail != null) txtEmail.Text = "";
                    if (txtSenha != null) txtSenha.Text = "";

                    formLoginOriginal.Show();
                }
                else
                {
                    // Se falhar por algum motivo, abre uma instância limpa
                    Form1 novoLogin = new Form1();
                    novoLogin.Show();
                }

                // 5. Fecha rigorosamente todos os outros formulários ativos (Menu, Perfil, etc.)
                List<Form> formulariosParaFechar = Application.OpenForms.Cast<Form>().ToList();
                foreach (Form frm in formulariosParaFechar)
                {
                    if (frm.Name != "Form1")
                    {
                        frm.Hide();
                        frm.Close();
                    }
                }
            }
        }
    }
}