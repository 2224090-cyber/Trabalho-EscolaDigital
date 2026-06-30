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

              
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox4.Enabled = false;
                button1.Enabled = false;
            }
            else
            {
                if (Conta.SaldoDevedor <= 0)
                {
                    label6.Text = "";
                    label7.Text = "";
                    label8.Text = "";
                }

                
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

            if (salario <= 0 || valorEmprestimo <= 0 || prazo <= 0)
            {
                MessageBox.Show("Valores inválidos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            if (prazo > 1080)
            {
                MessageBox.Show("O prazo máximo permitido é de 1080 meses (90 anos).", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal juros;

            if (poupanca >= 100000)
                juros = 0.02m;      // 2%
            else if (poupanca >= 75000)
                juros = 0.025m;     // 2,5%
            else if (poupanca >= 50000)
                juros = 0.03m;      // 3%
            else if (poupanca >= 40000)
                juros = 0.035m;     // 3,5%
            else if (poupanca >= 30000)
                juros = 0.04m;      // 4%
            else if (poupanca >= 20000)
                juros = 0.045m;     // 4,5%
            else if (poupanca >= 10000)
                juros = 0.05m;      // 5%
            else if (poupanca >= 5000)
                juros = 0.06m;      // 6%
            else if (poupanca >= 1000)
                juros = 0.07m;      // 7%
            else
                juros = 0.08m;      // 8%

            decimal valorTotal = valorEmprestimo + (valorEmprestimo * juros);
            Conta.ParcelaMensal = valorTotal / prazo;
            Conta.SaldoDevedor = valorTotal;

            label7.Text =
                $"Total a pagar: {Conta.SaldoDevedor:C}\n" +
                $"Parcela mensal: {Conta.ParcelaMensal:C}";

            if (Conta.ParcelaMensal <= salario * 0.30m)
            {
                Conta.EmprestimoAprovado = true;
                label8.Text = "Empréstimo APROVADO";
                label8.ForeColor = Color.Green;
            }
            else
            {
                Conta.EmprestimoAprovado = false;
                label8.Text = "Empréstimo NÃO APROVADO";
                label8.ForeColor = Color.Red;
            }
        }

     
        private void button2_Click(object sender, EventArgs e)
        {
            if (Conta.SaldoDevedor <= 0 || !Conta.EmprestimoAprovado || Conta.EmprestimoAtivo)
            {
                MessageBox.Show("Operação inválida ou empréstimo não aprovado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal valorEmprestimo = decimal.Parse(textBox2.Text);
            int totalParcelas = int.Parse(textBox4.Text);

            using (SqlConnection conexao = Database.GetConnection())
            {
                try
                {
                    conexao.Open();

                    
                    string queryUser = @"UPDATE Utilizadores 
                                         SET Saldo = Saldo + @ValorEmprestimo, 
                                             SaldoDevedor = @SaldoDevedor, 
                                             ParcelaMensal = @ParcelaMensal, 
                                             EmprestimoAtivo = 1 
                                         WHERE Id = @Id";

                    using (SqlCommand cmdUser = new SqlCommand(queryUser, conexao))
                    {
                        cmdUser.Parameters.AddWithValue("@ValorEmprestimo", valorEmprestimo);
                        cmdUser.Parameters.AddWithValue("@SaldoDevedor", Conta.SaldoDevedor);
                        cmdUser.Parameters.AddWithValue("@ParcelaMensal", Conta.ParcelaMensal);
                        cmdUser.Parameters.AddWithValue("@Id", Conta.Id);
                        cmdUser.ExecuteNonQuery();
                    }

                    
                    string queryEmp = @"INSERT INTO Emprestimos (UsuarioId, ValorSolicitado, ValorDevedor, ParcelaMensal, TotalParcelas, ParcelasPagas, Ativo) 
                                        VALUES (@UsuarioId, @ValorSolicitado, @ValorDevedor, @ParcelaMensal, @TotalParcelas, 0, 1)";

                    using (SqlCommand cmdEmp = new SqlCommand(queryEmp, conexao))
                    {
                        cmdEmp.Parameters.AddWithValue("@UsuarioId", Conta.Id);
                        cmdEmp.Parameters.AddWithValue("@ValorSolicitado", valorEmprestimo);
                        cmdEmp.Parameters.AddWithValue("@ValorDevedor", Conta.SaldoDevedor);
                        cmdEmp.Parameters.AddWithValue("@ParcelaMensal", Conta.ParcelaMensal);
                        cmdEmp.Parameters.AddWithValue("@TotalParcelas", totalParcelas);
                        cmdEmp.ExecuteNonQuery();
                    }

                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao salvar dados no banco de dados: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            MessageBox.Show("Empréstimo aceito e registado com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            menu_principal main = (menu_principal)Application.OpenForms["menu_principal"];
            if (main != null)
            {
                main.BuscarDadosDoBanco();
                main.AtualizarInterfaceCompleta();
            }

            AtualizarEmprestimo();
        }

        
        private void button3_Click(object sender, EventArgs e)
        {
            if (!Conta.EmprestimoAtivo)
            {
                MessageBox.Show("Não existe empréstimo ativo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal pagamento;
            if (!decimal.TryParse(textBox5.Text, out pagamento) || pagamento <= 0)
            {
                MessageBox.Show("Digite um valor de pagamento válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (pagamento > Conta.Saldo)
            {
                MessageBox.Show("Saldo insuficiente na conta.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal pagamentoReal = pagamento > Conta.SaldoDevedor ? Conta.SaldoDevedor : pagamento;
            decimal excedente = pagamento > Conta.SaldoDevedor ? pagamento - Conta.SaldoDevedor : 0;

            decimal novoSaldoDevedor = Conta.SaldoDevedor - pagamentoReal;
            bool novoStatusAtivo = novoSaldoDevedor > 0;
            decimal novaParcela = novoStatusAtivo ? Conta.ParcelaMensal : 0m;

            using (SqlConnection conexao = Database.GetConnection())
            {
                try
                {
                    conexao.Open();

                    
                    string queryUser = @"UPDATE Utilizadores 
                                         SET Saldo = Saldo - @PagamentoReal + @Excedente, 
                                             SaldoDevedor = @NovoSaldoDevedor, 
                                             ParcelaMensal = @NovaParcela, 
                                             EmprestimoAtivo = @NovoStatusAtivo 
                                         WHERE Id = @Id";

                    using (SqlCommand comando = new SqlCommand(queryUser, conexao))
                    {
                        comando.Parameters.AddWithValue("@PagamentoReal", pagamentoReal);
                        comando.Parameters.AddWithValue("@Excedente", excedente);
                        comando.Parameters.AddWithValue("@NovoSaldoDevedor", novoSaldoDevedor);
                        comando.Parameters.AddWithValue("@NovaParcela", novaParcela);
                        comando.Parameters.AddWithValue("@NovoStatusAtivo", novoStatusAtivo ? 1 : 0);
                        comando.Parameters.AddWithValue("@Id", Conta.Id);
                        comando.ExecuteNonQuery();
                    }

                    
                    string queryEmp = @"UPDATE Emprestimos 
                                         SET ValorDevedor = @NovoSaldoDevedor,
                                             ParcelasPagas = ParcelasPagas + 1,
                                             Ativo = @NovoStatusAtivo
                                         WHERE UsuarioId = @UsuarioId AND Ativo = 1";

                    using (SqlCommand cmdEmp = new SqlCommand(queryEmp, conexao))
                    {
                        cmdEmp.Parameters.AddWithValue("@NovoSaldoDevedor", novoSaldoDevedor);
                        cmdEmp.Parameters.AddWithValue("@NovoStatusAtivo", novoStatusAtivo ? 1 : 0);
                        cmdEmp.Parameters.AddWithValue("@UsuarioId", Conta.Id);
                        cmdEmp.ExecuteNonQuery();
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