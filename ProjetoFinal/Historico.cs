using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; // Adicionado para comunicação com o SQL Server

namespace Horazon_Bank__projetoFinal
{
    public partial class Historico : Form
    {
        public Historico()
        {
            InitializeComponent();
            Conta.ValoresAlterados += AtualizarHistorico;
        }

        private void Historico_VisibleChanged(object sender, EventArgs e)
        {
            AtualizarHistorico();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            AtualizarHistorico();
        }

        private void AtualizarHistorico()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(AtualizarHistorico));
                return;
            }

            StringBuilder sb = new StringBuilder();

            // Mostra o histórico invertido (o mais recente primeiro)
            for (int i = Conta.Historico.Count - 1; i >= 0; i--)
            {
                sb.AppendLine(Conta.Historico[i]);
            }

            // Se não houver nada, exibe uma mensagem amigável
            label2.Text = sb.Length > 0 ? sb.ToString() : "Nenhuma transação registada até ao momento.";
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        // ===================== LIMPAR HISTÓRICO (SQL + RAM) =====================
        private void button1_Click(object sender, EventArgs e)
        {
            if (Conta.Historico.Count == 0)
            {
                MessageBox.Show("O histórico já está vazio.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult resposta = MessageBox.Show(
                "Tem a certeza que deseja limpar todo o seu histórico de transações?",
                "Confirmar Limpeza",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resposta == DialogResult.Yes)
            {
                // 1. LIMPAR NA BASE DE DADOS (SQL SERVER)
                using (SqlConnection conexao = Database.GetConnection())
                {
                    try
                    {
                        conexao.Open();
                        string queryLimpar = "DELETE FROM HistoricoTransacoes WHERE UsuarioId = @UsuarioId";

                        using (SqlCommand cmd = new SqlCommand(queryLimpar, conexao))
                        {
                            cmd.Parameters.AddWithValue("@UsuarioId", Conta.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao limpar histórico no servidor: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Aborta para não limpar a RAM se falhar no banco
                    }
                }

                // 2. LIMPAR NA MEMÓRIA RAM
                Conta.LimparHistorico();

                MessageBox.Show("Histórico limpo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}