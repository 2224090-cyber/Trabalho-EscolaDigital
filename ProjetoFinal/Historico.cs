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
    public partial class Historico : Form
    {
        public Historico()
        {
            InitializeComponent();
            Conta.ValoresAlterados += AtualizarHistorico;
        }

        private void Historico_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                CarregarHistoricoDoBanco();
                AtualizarHistorico();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            CarregarHistoricoDoBanco();
            AtualizarHistorico();
        }

        
        private void CarregarHistoricoDoBanco()
        {
            if (string.IsNullOrEmpty(Conta.Id)) return;

            
            Conta.Historico.Clear();

            
            string query = "SELECT Texto FROM HistoricoTransacoes WHERE UsuarioId = @UsuarioId ORDER BY Id ASC";

            try
            {
                using (SqlConnection conexao = Database.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conexao))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioId", Conta.Id);
                        conexao.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string linha = reader["Texto"].ToString();
                                Conta.Historico.Add(linha);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar histórico do banco de dados: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

      
        private void AtualizarHistorico()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(AtualizarHistorico));
                return;
            }

            StringBuilder sb = new StringBuilder();

          
            for (int i = Conta.Historico.Count - 1; i >= 0; i--)
            {
                
                if (Conta.Historico[i].Contains("[OCULTO_CLIENTE]"))
                {
                    break;
                }

                sb.AppendLine(Conta.Historico[i]);
            }

            
            label2.Text = sb.Length > 0 ? sb.ToString() : "Nenhuma transação registada até ao momento.";
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }

       
        private void button1_Click(object sender, EventArgs e)
        {
            if (label2.Text == "Nenhuma transação registada até ao momento.")
            {
                MessageBox.Show("O seu histórico já está vazio.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult resposta = MessageBox.Show(
                "Tem a certeza que deseja limpar a visualização do seu histórico de transações?\n\n" +
                "(Nota de Segurança: O banco guardará o registo das transações para fins de auditoria).",
                "Confirmar Limpeza",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resposta == DialogResult.Yes)
            {
                
                using (SqlConnection conexao = Database.GetConnection())
                {
                    try
                    {
                        conexao.Open();
                        string queryMarcarLimpo = "INSERT INTO HistoricoTransacoes (UsuarioId, Texto) VALUES (@UsuarioId, @Texto)";

                        using (SqlCommand cmd = new SqlCommand(queryMarcarLimpo, conexao))
                        {
                            cmd.Parameters.AddWithValue("@UsuarioId", Conta.Id);
                            cmd.Parameters.AddWithValue("@Texto", $"[{DateTime.Now:dd/MM/yyyy HH:mm}] [OCULTO_CLIENTE] Histórico limpo pelo utilizador.");

                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro de comunicação com o servidor: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                
                CarregarHistoricoDoBanco();
                AtualizarHistorico();

                MessageBox.Show("Histórico de visualização limpo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}