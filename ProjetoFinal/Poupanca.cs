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
    public partial class Poupanca : Form
    {
        public Poupanca()
        {
            InitializeComponent();
            Conta.ValoresAlterados += AtualizarValores;
        }

        private void Poupanca_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                AtualizarValores();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            AtualizarValores();
        }

        private void Poupanca_Load(object sender, EventArgs e)
        {
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

        // =========================================================================
        // --- AÇÃO PRINCIPAL: CONFIRMAR OPERAÇÃO (GUARDAR OU RETIRAR) ---
        // =========================================================================
        private void button1_Click(object sender, EventArgs e)
        {
            decimal guardar = 0;
            decimal retirar = 0;

            bool querGuardar = !string.IsNullOrWhiteSpace(textBox1.Text);
            bool querRetirar = !string.IsNullOrWhiteSpace(textBox2.Text);

            if (!querGuardar && !querRetirar)
            {
                MessageBox.Show("Digite um valor.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (querGuardar && querRetirar)
            {
                MessageBox.Show("Preencha apenas um campo de cada vez.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string txtHistorico = "";

            // --- LÓGICA: GUARDAR DINHEIRO ---
            if (querGuardar)
            {
                if (!decimal.TryParse(textBox1.Text, out guardar) || guardar <= 0)
                {
                    MessageBox.Show("Valor inválido para guardar.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (guardar > Conta.Saldo)
                {
                    MessageBox.Show("Saldo insuficiente na conta corrente.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Conta.Saldo -= guardar;
                Conta.Poupanca += guardar;
                txtHistorico = $"Poupança: Transferido para poupança -{guardar:C}";
            }

            // --- LÓGICA: RETIRAR DINHEIRO ---
            if (querRetirar)
            {
                if (!decimal.TryParse(textBox2.Text, out retirar) || retirar <= 0)
                {
                    MessageBox.Show("Valor inválido para retirar.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (retirar > Conta.Poupanca)
                {
                    MessageBox.Show("Saldo insuficiente na poupança.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Conta.Poupanca -= retirar;
                Conta.Saldo += retirar;
                txtHistorico = $"Poupança: Resgatado da poupança +{retirar:C}";
            }

            // --- ATUALIZAR NO SQL SERVER ---
            using (SqlConnection conexao = Database.GetConnection())
            {
                try
                {
                    conexao.Open();

                    // 1. Atualizar Saldos do Utilizador
                    string querySaldos = @"UPDATE Utilizadores 
                                           SET Saldo = @Saldo, Poupanca = @Poupanca 
                                           WHERE Id = @Id";

                    using (SqlCommand cmdSaldos = new SqlCommand(querySaldos, conexao))
                    {
                        cmdSaldos.Parameters.AddWithValue("@Saldo", Conta.Saldo);
                        cmdSaldos.Parameters.AddWithValue("@Poupanca", Conta.Poupanca);
                        cmdSaldos.Parameters.AddWithValue("@Id", Conta.Id);
                        cmdSaldos.Parameters.AddWithValue("@Texto", txtHistorico);
                        cmdSaldos.ExecuteNonQuery();
                    }

                    // 2. Registar permanentemente na tabela HistoricoTransacoes
                    string queryHistorico = @"INSERT INTO HistoricoTransacoes (UsuarioId, Texto) 
                                              VALUES (@UsuarioId, @Texto)";

                    using (SqlCommand cmdHist = new SqlCommand(queryHistorico, conexao))
                    {
                        cmdHist.Parameters.AddWithValue("@UsuarioId", Conta.Id);
                        cmdHist.Parameters.AddWithValue("@Texto", txtHistorico);
                        cmdHist.ExecuteNonQuery();
                    }

                    Conta.AdicionarHistorico(txtHistorico);
                    MessageBox.Show("Operação realizada e sincronizada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // ✅ CORREÇÃO: Avisa o menu_principal para atualizar as Labels de Saldo e Poupança imediatamente
                    menu_principal main = (menu_principal)Application.OpenForms["menu_principal"];
                    if (main != null)
                    {
                        main.BuscarDadosDoBanco();
                        main.AtualizarInterfaceCompleta();
                    }
                }
                catch (Exception ex)
                {
                    // REVERSÃO DE SEGURANÇA
                    if (querGuardar)
                    {
                        Conta.Saldo += guardar;
                        Conta.Poupanca -= guardar;
                    }
                    else if (querRetirar)
                    {
                        Conta.Poupanca += retirar;
                        Conta.Saldo -= retirar;
                    }

                    MessageBox.Show("Erro ao salvar operação no banco de dados. Ação cancelada.\nDetalhes: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Limpa os campos da interface
            System.Diagnostics.Debug.WriteLine($"Saldo atual: {Conta.Saldo}, Poupança: {Conta.Poupanca}");
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
            AtualizarValores();
        }
    }
}