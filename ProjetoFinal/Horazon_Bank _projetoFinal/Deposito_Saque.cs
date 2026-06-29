using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient; // Adicionado para comandos SQL Server
using System.Windows.Forms;

namespace Horazon_Bank__projetoFinal
{
    public partial class Deposito_Saque : Form
    {
        public Deposito_Saque()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }

        // ===================== BOTÃO DE DEPÓSITO (button1) =====================
        private void button1_Click(object sender, EventArgs e)
        {
            decimal valor;

            if (!decimal.TryParse(textBox1.Text, out valor))
            {
                MessageBox.Show("Digite um valor válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (valor <= 0)
            {
                MessageBox.Show("O valor deve ser maior que zero.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. Atualizar a Base de Dados (SQL Server)
            using (SqlConnection conexao = Database.GetConnection())
            {
                try
                {
                    conexao.Open();
                    // Incrementa o valor digitado diretamente no Saldo do utilizador
                    string query = "UPDATE Utilizadores SET Saldo = Saldo + @Valor WHERE Id = @Id";

                    using (SqlCommand comando = new SqlCommand(query, conexao))
                    {
                        comando.Parameters.AddWithValue("@Valor", valor);
                        comando.Parameters.AddWithValue("@Id", Conta.Id);

                        comando.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao salvar o depósito na base de dados: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Cancela se der erro no banco
                }
            }

            // 2. Atualiza a classe estática e histórico local
            Conta.Saldo += valor;
            Conta.AdicionarHistorico($"Depósito: +{valor:C}");

            MessageBox.Show("Depósito realizado com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            textBox1.Clear();

            // 3. ✅ ATUALIZAR O MENU PRINCIPAL INSTANTANEAMENTE
            menu_principal main = (menu_principal)Application.OpenForms["menu_principal"];
            if (main != null)
            {
                main.BuscarDadosDoBanco();
                main.AtualizarInterfaceCompleta();
            }
        }

        // ===================== BOTÃO DE SAQUE / LEVANTAMENTO (button2) =====================
        private void button2_Click(object sender, EventArgs e)
        {
            decimal valor;

            if (!decimal.TryParse(textBox2.Text, out valor))
            {
                MessageBox.Show("Digite um valor válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (valor <= 0)
            {
                MessageBox.Show("O valor deve ser maior que zero.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (valor > Conta.Saldo)
            {
                MessageBox.Show("Saldo insuficiente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. Atualizar a Base de Dados (SQL Server)
            using (SqlConnection conexao = Database.GetConnection())
            {
                try
                {
                    conexao.Open();
                    // Subtrai o valor digitado diretamente no Saldo do utilizador
                    string query = "UPDATE Utilizadores SET Saldo = Saldo - @Valor WHERE Id = @Id";

                    using (SqlCommand comando = new SqlCommand(query, conexao))
                    {
                        comando.Parameters.AddWithValue("@Valor", valor);
                        comando.Parameters.AddWithValue("@Id", Conta.Id);

                        comando.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao salvar o saque na base de dados: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Cancela se der erro no banco
                }
            }

            // 2. Atualiza a classe estática e histórico local
            Conta.Saldo -= valor;
            Conta.AdicionarHistorico($"Saque: -{valor:C}");

            MessageBox.Show("Saque realizado com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            textBox2.Clear();

            // 3. ✅ ATUALIZAR O MENU PRINCIPAL INSTANTANEAMENTE
            menu_principal main = (menu_principal)Application.OpenForms["menu_principal"];
            if (main != null)
            {
                main.BuscarDadosDoBanco();
                main.AtualizarInterfaceCompleta();
            }
        }
    }
}
