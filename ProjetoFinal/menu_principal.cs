using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Horazon_Bank__projetoFinal
{
    public partial class menu_principal : Form
    {
        private Form frmAtivo;

        public menu_principal()
        {
            InitializeComponent();
        }

        // ===================== SINCRONIZAÇÃO COM SQL SERVER =====================
        // Sincroniza os dados, mas agora a interface só vai refletir o Saldo
        public void BuscarDadosDoBanco()
        {
            string contaId = Conta.Id;

            if (string.IsNullOrEmpty(contaId)) return;

            string query = "SELECT Saldo, Poupanca FROM Utilizadores WHERE Id = @Id";

            try
            {
                using (SqlConnection conexao = Database.GetConnection())
                {
                    using (SqlCommand comando = new SqlCommand(query, conexao))
                    {
                        comando.Parameters.AddWithValue("@Id", contaId);

                        conexao.Open();
                        using (SqlDataReader reader = comando.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Conta.Saldo = Convert.ToDecimal(reader["Saldo"]);
                                Conta.Poupanca = Convert.ToDecimal(reader["Poupanca"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao sincronizar com a base de dados: {ex.Message}",
                                "Erro de Conexão", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ===================== ATUALIZAÇÃO DA INTERFACE =====================
        // ✅ CORRIGIDO: Este método agora ATUALIZA APENAS O SALDO (label2). A label1 nunca é tocada!
        public void AtualizarInterfaceCompleta()
        {
            label2.Text = $"Saldo: {Conta.Saldo:C}";
        }

        public void AtualizarSaldo()
        {
            label2.Text = $"Saldo: {Conta.Saldo:C}";
        }

        // ✅ REMOVIDO: O método AtualizarPoupanca() que alterava a label1 foi eliminado.

        public void AtualizarHistorico() { }

        // ===================== EVENTOS DE CARREGAMENTO =====================
        private void Menu_Principal_Load(object sender, EventArgs e)
        {
            BuscarDadosDoBanco();
            AtualizarInterfaceCompleta(); // Atualiza apenas o Saldo
        }

        private void Menu_Principal_Activated(object sender, EventArgs e)
        {
            BuscarDadosDoBanco();
            AtualizarInterfaceCompleta(); // Atualiza apenas o Saldo
        }

        // ===================== GESTÃO DE SUBFORMULÁRIOS =====================
        private void FormShow(Form frm)
        {
            ActiveFormClose();
            frmAtivo = frm;
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            panel3.Controls.Add(frm);
            panel3.Tag = frm;
            frm.BringToFront();
            frm.Show();
        }

        private void ActiveFormClose()
        {
            if (frmAtivo != null)
                frmAtivo.Close();
        }

        private void ActiveButton(Button btnAtivo)
        {
            foreach (Control ctrl in panel1.Controls)
            {
                if (ctrl is Button)
                    ctrl.ForeColor = Color.White;
            }
            btnAtivo.ForeColor = Color.Red;
        }

        // ===================== CLIQUES DOS BOTÕES =====================
        private void button1_Click(object sender, EventArgs e) { }
        private void panel3_Paint(object sender, PaintEventArgs e) { }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ActiveButton(button1);
            ActiveFormClose();
            BuscarDadosDoBanco();
            AtualizarInterfaceCompleta(); // Atualiza apenas o Saldo ao clicar no Início
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ActiveButton(button2);
            FormShow(new Deposito_Saque());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ActiveButton(button3);
            FormShow(new Transferir());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ActiveButton(button4);
            FormShow(new Emprestimos());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ActiveButton(button5);
            FormShow(new Poupanca());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ActiveButton(button6);
            FormShow(new Conversor_moedas());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ActiveButton(button7);
            FormShow(new Historico());
        }

        private void button8_Click(object sender, EventArgs e) => Application.Exit();

        private void button9_Click(object sender, EventArgs e)
        {
            ActiveButton(button9);
            FormShow(new perfil());
        }

        private void label2_Click(object sender, EventArgs e)
        {
            BuscarDadosDoBanco();
            AtualizarInterfaceCompleta();
        }

        private void label1_Click(object sender, EventArgs e) { }
    }
}