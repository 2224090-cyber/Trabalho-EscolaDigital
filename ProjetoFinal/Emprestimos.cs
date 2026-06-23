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
using System.Data.SqlClient; // Adicionado para suportar comandos SQL Server

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
    public partial class Emprestimos : Form
    {
        public Emprestimos()
        {
            InitializeComponent();
            Conta.ValoresAlterados += AtualizarValores;
        }

        private void CarregarDadosEmprestimoDoBanco()
        {
            string query = "SELECT SaldoDevedor, ParcelaMensal, EmprestimoAtivo FROM Utilizadores WHERE Id = @Id";

            try
            {
                using (SqlConnection conexao = Database.GetConnection())
                {
                    using (SqlCommand comando = new SqlCommand(query, conexao))
                    {
                        comando.Parameters.AddWithValue("@Id", Conta.Id);
                        conexao.Open();

                        using (SqlDataReader reader = comando.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Conta.SaldoDevedor = reader["SaldoDevedor"] != DBNull.Value ? Convert.ToDecimal(reader["SaldoDevedor"]) : 0m;
                                Conta.ParcelaMensal = reader["ParcelaMensal"] != DBNull.Value ? Convert.ToDecimal(reader["ParcelaMensal"]) : 0m;
                                Conta.EmprestimoAtivo = reader["EmprestimoAtivo"] != DBNull.Value && Convert.ToBoolean(reader["EmprestimoAtivo"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados do empréstimo: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

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
                // ✅ MODIFICADO: Se não houver empréstimo ativo, mantém o texto que o botão 1 gerou.
                // Só limpa se o Saldo Devedor for realmente zero.
                if (Conta.SaldoDevedor <= 0)
                {
                    label6.Text = "";
                    label7.Text = "";
                    label8.Text = "";
                }

                // Desbloquear campos
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox4.Enabled = true;
                button1.Enabled = true;
            }
        }

        private void FormEmprestimo_Load(object sender, EventArgs e)
        {
            CarregarDadosEmprestimoDoBanco();
            AtualizarEmprestimo();
        }

        private void Emprestimos_Load(object sender, EventArgs e)
        {
            CarregarDadosEmprestimoDoBanco();
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
            if (this.Visible)
            {
                CarregarDadosEmprestimoDoBanco();
                AtualizarEmprestimo();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            CarregarDadosEmprestimoDoBanco();
            AtualizarEmprestimo();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Conta.ValoresAlterados -= AtualizarValores;
            base.OnFormClosed(e);
        }

        // ===================== BOTÃO 1: CALCULAR / SIMULAR EMPRÉSTIMO =====================
        private void button1_Click(object sender, EventArgs e)
        {
            if (Conta.EmprestimoAtivo)
            {
                MessageBox.Show("Já existe um empréstimo ativo. Quite-o antes de solicitar outro.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Preencha todos os campos obrigatórios.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("Digite apenas números válidos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (salario <= 0 || valorEmprestimo <= 0 || prazo <= 0)
            {
                MessageBox.Show("Valores inválidos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal juros;

            if (poupanca >= 10000)
                juros = 0.05m;
            else if (poupanca >= 5000)
                juros = 0.10m;
            else
                juros = 0.15m;

            // Cálculos
            decimal valorTotal = valorEmprestimo + (valorEmprestimo * juros);
            Conta.ParcelaMensal = valorTotal / prazo;
            Conta.SaldoDevedor = valorTotal;

            // ✅ CORREÇÃO: Força a atualização do texto do ecrã no exato momento do clique
            label7.Text =
                $"Total a pagar: {Conta.SaldoDevedor:C}\n" +
                $"Parcela mensal: {Conta.ParcelaMensal:C}";

            if (Conta.ParcelaMensal <= salario * 0.30m)
            {
                Conta.EmprestimoAprovado = true;
                label8.Text = "Empréstimo APROVADO";
                label8.ForeColor = Color.Green; // Opcional: Feedback visual positivo
            }
            else
            {
                Conta.EmprestimoAprovado = false;
                label8.Text = "Empréstimo NÃO APROVADO";
                label8.ForeColor = Color.Red; // Opcional: Feedback visual negativo
            }
        }

        // ===================== BOTÃO 2: ACEITAR EMPRÉSTIMO =====================
        private void button2_Click(object sender, EventArgs e)
        {
            if (Conta.SaldoDevedor <= 0)
            {
                MessageBox.Show("Nenhum empréstimo válido foi calculado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Conta.EmprestimoAprovado)
            {
                MessageBox.Show("Empréstimo não aprovado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Conta.EmprestimoAtivo)
            {
                MessageBox.Show("Já existe um empréstimo ativo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal valorEmprestimo = decimal.Parse(textBox2.Text);

            using (SqlConnection conexao = Database.GetConnection())
            {
                try
                {
                    conexao.Open();
                    string query = @"UPDATE Utilizadores 
                                     SET Saldo = Saldo + @ValorEmprestimo, 
                                         SaldoDevedor = @SaldoDevedor, 
                                         ParcelaMensal = @ParcelaMensal, 
                                         EmprestimoAtivo = 1 
                                     WHERE Id = @Id";

                    using (SqlCommand comando = new SqlCommand(query, conexao))
                    {
                        comando.Parameters.AddWithValue("@ValorEmprestimo", valorEmprestimo);
                        comando.Parameters.AddWithValue("@SaldoDevedor", Conta.SaldoDevedor);
                        comando.Parameters.AddWithValue("@ParcelaMensal", Conta.ParcelaMensal);
                        comando.Parameters.AddWithValue("@Id", Conta.Id);

                        comando.ExecuteNonQuery();
                    }

                    string queryHist = "INSERT INTO HistoricoTransacoes (UsuarioId, Texto) VALUES (@Id, @Texto)";
                    using (SqlCommand cmdHist = new SqlCommand(queryHist, conexao))
                    {
                        cmdHist.Parameters.AddWithValue("@Id", Conta.Id);
                        cmdHist.Parameters.AddWithValue("@Texto", $"Empréstimo: +{valorEmprestimo:C}");
                        cmdHist.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao salvar empréstimo no banco de dados: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            Conta.Saldo += valorEmprestimo;
            Conta.AdicionarHistorico($"Empréstimo: +{valorEmprestimo:C}");
            Conta.EmprestimoAtivo = true;

            label6.Text = "Empréstimo Ativo";

            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox4.Enabled = false;
            button1.Enabled = false;

            MessageBox.Show("Empréstimo aceito com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            menu_principal main = (menu_principal)Application.OpenForms["menu_principal"];
            if (main != null)
            {
                main.BuscarDadosDoBanco();
                main.AtualizarInterfaceCompleta();
            }

            AtualizarEmprestimo();
        }

        // ===================== BOTÃO 3: PAGAR / AMORTIZAR EMPRÉSTIMO =====================
        private void button3_Click(object sender, EventArgs e)
        {
            if (!Conta.EmprestimoAtivo)
            {
                MessageBox.Show("Não existe empréstimo ativo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal pagamento;

            if (!decimal.TryParse(textBox5.Text, out pagamento))
            {
                MessageBox.Show("Digite um valor válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (pagamento <= 0)
            {
                MessageBox.Show("O pagamento deve ser maior que zero.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (pagamento > Conta.Saldo)
            {
                MessageBox.Show("Saldo insuficiente na conta.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal pagamentoReal = pagamento;
            decimal excedente = 0;

            if (pagamento > Conta.SaldoDevedor)
            {
                excedente = pagamento - Conta.SaldoDevedor;
                pagamentoReal = Conta.SaldoDevedor;
            }

            decimal novoSaldoDevedor = Conta.SaldoDevedor - pagamentoReal;
            bool novoStatusAtivo = novoSaldoDevedor > 0;
            decimal novaParcela = novoStatusAtivo ? Conta.ParcelaMensal : 0m;

            using (SqlConnection conexao = Database.GetConnection())
            {
                try
                {
                    conexao.Open();
                    string query = @"UPDATE Utilizadores 
                                     SET Saldo = Saldo - @PagamentoReal + @Excedente, 
                                         SaldoDevedor = @NovoSaldoDevedor, 
                                         ParcelaMensal = @NovaParcela, 
                                         EmprestimoAtivo = @NovoStatusAtivo 
                                     WHERE Id = @Id";

                    using (SqlCommand comando = new SqlCommand(query, conexao))
                    {
                        comando.Parameters.AddWithValue("@PagamentoReal", pagamentoReal);
                        comando.Parameters.AddWithValue("@Excedente", excedente);
                        comando.Parameters.AddWithValue("@NovoSaldoDevedor", novoSaldoDevedor);
                        comando.Parameters.AddWithValue("@NovaParcela", novaParcela);
                        comando.Parameters.AddWithValue("@NovoStatusAtivo", novoStatusAtivo ? 1 : 0);
                        comando.Parameters.AddWithValue("@Id", Conta.Id);

                        comando.ExecuteNonQuery();
                    }

                    string txtHist = novoStatusAtivo ? $"Pagamento do empréstimo: -{pagamentoReal:C}" : "Empréstimo quitado";
                    string queryHist = "INSERT INTO HistoricoTransacoes (UsuarioId, Texto) VALUES (@Id, @Texto)";
                    using (SqlCommand cmdHist = new SqlCommand(queryHist, conexao))
                    {
                        cmdHist.Parameters.AddWithValue("@Id", Conta.Id);
                        cmdHist.Parameters.AddWithValue("@Texto", txtHist);
                        cmdHist.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao processar pagamento na base de dados: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (excedente > 0)
            {
                MessageBox.Show($"Você pagou {excedente:C} a mais. Esse valor será devolvido à sua conta.", "Reembolso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Conta.Saldo -= pagamentoReal;
            Conta.Saldo += excedente;
            Conta.SaldoDevedor = novoSaldoDevedor;
            Conta.AdicionarHistorico($"Pagamento do empréstimo: -{pagamentoReal:C}");

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

                MessageBox.Show("Empréstimo quitado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                label7.Text =
                    $"Total a pagar: {Conta.SaldoDevedor:C}\n" +
                    $"Parcela mensal: {Conta.ParcelaMensal:C}";

                textBox5.Clear();
                MessageBox.Show("Pagamento realizado com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            menu_principal main = (menu_principal)Application.OpenForms["menu_principal"];
            if (main != null)
            {
                main.BuscarDadosDoBanco();
                main.AtualizarInterfaceCompleta();
            }

            AtualizarEmprestimo();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
    }
}