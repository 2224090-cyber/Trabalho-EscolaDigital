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
using System.Data.SqlClient; // Suporte comandos SQL Server

namespace Horazon_Bank__projetoFinal
{
    public partial class Transferir : Form
    {
        public Transferir()
        {
            InitializeComponent();

            // Força o "Por ID" a começar selecionado para evitar confusões na busca
            radioButton1.Checked = true;
            
        }

        // =========================================================================
        // === BOTÃO 1: CONFIRMAR A TRANSFERÊNCIA ==================================
        // =========================================================================
        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Validações iniciais de campos vazios
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Validação do Valor
            decimal valorTransferencia;
            if (!decimal.TryParse(textBox2.Text, out valorTransferencia) || valorTransferencia <= 0)
            {
                MessageBox.Show("Digite um valor de transferência válido e maior que zero.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. Verifica se o utilizador tem saldo suficiente
            if (valorTransferencia > Conta.Saldo)
            {
                MessageBox.Show($"Saldo insuficiente! O teu saldo atual é: {Conta.Saldo:C}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string buscaDestinatario = textBox1.Text.Trim();
            string destinatarioId = "";
            string destinatarioNome = "";

            // 4. Conexão à Base de Dados para procurar o Destinatário e Executar a Transação
            using (SqlConnection conexao = Database.GetConnection())
            {
                try
                {
                    conexao.Open();

                    // --- PASSO A: Descobrir o ID e Nome do Destinatário ---
                    // Mudamos de '=' para 'LIKE' para evitar problemas caso a coluna no SQL seja CHAR (que adiciona espaços em branco ocultos)
                    string queryBusca = radioButton1.Checked
                        ? "SELECT Id, Nome FROM Utilizadores WHERE Id LIKE @Busca"
                        : "SELECT Id, Nome FROM Utilizadores WHERE Email LIKE @Busca";

                    using (SqlCommand cmdBusca = new SqlCommand(queryBusca, conexao))
                    {
                        // O Trim() remove espaços do início e fim. O parâmetro garante segurança contra SQL Injection.
                        cmdBusca.Parameters.AddWithValue("@Busca", buscaDestinatario);

                        using (SqlDataReader reader = cmdBusca.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                destinatarioId = reader["Id"].ToString().Trim();
                                destinatarioNome = reader["Nome"].ToString().Trim();
                            }
                            else
                            {
                                // Fechar o reader para conseguir rodar uma query de diagnóstico na mesma conexão
                                reader.Close();

                                // DIAGNÓSTICO: Vamos ver quantos utilizadores existem mesmo no banco para perceber o problema
                                int totalUsuarios = 0;
                                using (SqlCommand cmdCount = new SqlCommand("SELECT COUNT(*) FROM Utilizadores", conexao))
                                {
                                    totalUsuarios = (int)cmdCount.ExecuteScalar();
                                }

                                string tipo = radioButton1.Checked ? "ID" : "E-mail";
                                MessageBox.Show(
                                    $"Não foi encontrado nenhum utilizador com o {tipo}: '{buscaDestinatario}'.\n\n" +
                                    $"[Dados de Diagnóstico]:\n" +
                                    $"- Total de utilizadores cadastrados no banco: {totalUsuarios}\n" +
                                    $"- O seu ID atual logado é: '{Conta.Id}'\n\n" +
                                    $"Se o total de utilizadores for apenas 1, significa que os outros registos não guardaram ou está noutra base de dados.",
                                    "Erro de Busca e Diagnóstico", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                        }
                    }

                    // Impedir que o utilizador transfira para si mesmo
                    if (destinatarioId == Conta.Id)
                    {
                        MessageBox.Show("Não pode fazer uma transferência para si mesmo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // --- PASSO B: Executar os descontos e acréscimos (Processo Bancário) ---
                    // 1. Retira dinheiro do Remetente (Utilizador Logado)
                    string queryRemetente = "UPDATE Utilizadores SET Saldo = Saldo - @Valor WHERE Id = @Id";
                    using (SqlCommand cmdRem = new SqlCommand(queryRemetente, conexao))
                    {
                        cmdRem.Parameters.AddWithValue("@Valor", valorTransferencia);
                        cmdRem.Parameters.AddWithValue("@Id", Conta.Id);
                        cmdRem.ExecuteNonQuery();
                    }

                    // 2. Adiciona dinheiro ao Destinatário
                    string queryDestinatario = "UPDATE Utilizadores SET Saldo = Saldo + @Valor WHERE Id = @Id";
                    using (SqlCommand cmdDest = new SqlCommand(queryDestinatario, conexao))
                    {
                        cmdDest.Parameters.AddWithValue("@Valor", valorTransferencia);
                        cmdDest.Parameters.AddWithValue("@Id", destinatarioId);
                        cmdDest.ExecuteNonQuery();
                    }

                    // --- PASSO C: Registar na tabela oficial de Transferencias ---
                    string queryTransf = @"INSERT INTO Transferencias (RemetenteId, DestinatarioId, Valor) 
                                           VALUES (@RemetenteId, @DestinatarioId, @Valor)";
                    using (SqlCommand cmdTransf = new SqlCommand(queryTransf, conexao))
                    {
                        cmdTransf.Parameters.AddWithValue("@RemetenteId", Conta.Id);
                        cmdTransf.Parameters.AddWithValue("@DestinatarioId", destinatarioId);
                        cmdTransf.Parameters.AddWithValue("@Valor", valorTransferencia);
                        cmdTransf.ExecuteNonQuery();
                    }

                    // --- PASSO D: Gravar nas tabelas de histórico de ambos ---
                    string dataHoraStr = $"[{DateTime.Now:dd/MM/yyyy HH:mm}]";
                    string queryHist = "INSERT INTO HistoricoTransacoes (UsuarioId, Texto) VALUES (@UsuarioId, @Texto)";

                    string tipoReferencia = radioButton1.Checked ? "ID" : "E-mail";

                    // Histórico de quem enviou (Remetente)
                    using (SqlCommand cmdHist1 = new SqlCommand(queryHist, conexao))
                    {
                        cmdHist1.Parameters.AddWithValue("@UsuarioId", Conta.Id);
                        cmdHist1.Parameters.AddWithValue("@Texto", $"{dataHoraStr} Transf. Enviada para o {tipoReferencia} {buscaDestinatario}: -{valorTransferencia:C}");
                        cmdHist1.ExecuteNonQuery();
                    }

                    // Histórico de quem recebeu (Destinatário)
                    using (SqlCommand cmdHist2 = new SqlCommand(queryHist, conexao))
                    {
                        cmdHist2.Parameters.AddWithValue("@UsuarioId", destinatarioId);
                        cmdHist2.Parameters.AddWithValue("@Texto", $"{dataHoraStr} Transf. Recebida do ID {Conta.Id}: +{valorTransferencia:C}");
                        cmdHist2.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro crítico ao processar transferência: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // 5. Atualizar a memória RAM da classe Conta local
            Conta.Saldo -= valorTransferencia;
            string tipoRefLocal = radioButton1.Checked ? "ID" : "E-mail";
            Conta.Historico.Add($"[{DateTime.Now:dd/MM/yyyy HH:mm}] Transf. Enviada para o {tipoRefLocal} {buscaDestinatario}: -{valorTransferencia:C}");

            // 6. Limpar os campos e dar feedback de sucesso
            textBox1.Clear();
            textBox2.Clear();
            MessageBox.Show($"Transferência de {valorTransferencia:C} enviada com sucesso para {destinatarioNome}!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 7. Sincronizar o Menu Principal se estiver aberto
            menu_principal main = (menu_principal)Application.OpenForms["menu_principal"];
            if (main != null)
            {
                main.BuscarDadosDoBanco();
                main.AtualizarInterfaceCompleta();
            }
        }

        // =========================================================================
        // === EVENTOS DE MUDANÇA DOS RADIO BUTTONS (Dicas visuais na label2) ======
        // =========================================================================
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                label2.Text = "Digite o ID do destinatário:";
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                label2.Text = "Digite o E-mail do destinatário:";
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            // Pode ficar vazio
        }
    }
}