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
    public partial class Transferir : Form
    {
        public Transferir()
        {
            InitializeComponent();

          
            radioButton1.Checked = true;
            
        }

       
        private void button1_Click(object sender, EventArgs e)
        {
           
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

           
            decimal valorTransferencia;
            if (!decimal.TryParse(textBox2.Text, out valorTransferencia) || valorTransferencia <= 0)
            {
                MessageBox.Show("Digite um valor de transferência válido e maior que zero.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            if (valorTransferencia > Conta.Saldo)
            {
                MessageBox.Show($"Saldo insuficiente! O teu saldo atual é: {Conta.Saldo:C}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string buscaDestinatario = textBox1.Text.Trim();
            string destinatarioId = "";
            string destinatarioNome = "";

            using (SqlConnection conexao = Database.GetConnection())
            {
                try
                {
                    conexao.Open();

                   
                    string queryBusca = radioButton1.Checked
                        ? "SELECT Id, Nome FROM Utilizadores WHERE Id LIKE @Busca"
                        : "SELECT Id, Nome FROM Utilizadores WHERE Email LIKE @Busca";

                    using (SqlCommand cmdBusca = new SqlCommand(queryBusca, conexao))
                    {
                        
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
                               
                                reader.Close();

                                
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

                    
                    if (destinatarioId == Conta.Id)
                    {
                        MessageBox.Show("Não pode fazer uma transferência para si mesmo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                   
                    string queryRemetente = "UPDATE Utilizadores SET Saldo = Saldo - @Valor WHERE Id = @Id";
                    using (SqlCommand cmdRem = new SqlCommand(queryRemetente, conexao))
                    {
                        cmdRem.Parameters.AddWithValue("@Valor", valorTransferencia);
                        cmdRem.Parameters.AddWithValue("@Id", Conta.Id);
                        cmdRem.ExecuteNonQuery();
                    }

                    
                    string queryDestinatario = "UPDATE Utilizadores SET Saldo = Saldo + @Valor WHERE Id = @Id";
                    using (SqlCommand cmdDest = new SqlCommand(queryDestinatario, conexao))
                    {
                        cmdDest.Parameters.AddWithValue("@Valor", valorTransferencia);
                        cmdDest.Parameters.AddWithValue("@Id", destinatarioId);
                        cmdDest.ExecuteNonQuery();
                    }

                    
                    string queryTransf = @"INSERT INTO Transferencias (RemetenteId, DestinatarioId, Valor) 
                                           VALUES (@RemetenteId, @DestinatarioId, @Valor)";
                    using (SqlCommand cmdTransf = new SqlCommand(queryTransf, conexao))
                    {
                        cmdTransf.Parameters.AddWithValue("@RemetenteId", Conta.Id);
                        cmdTransf.Parameters.AddWithValue("@DestinatarioId", destinatarioId);
                        cmdTransf.Parameters.AddWithValue("@Valor", valorTransferencia);
                        cmdTransf.ExecuteNonQuery();
                    }

                   
                    string dataHoraStr = $"[{DateTime.Now:dd/MM/yyyy HH:mm}]";
                    string queryHist = "INSERT INTO HistoricoTransacoes (UsuarioId, Texto) VALUES (@UsuarioId, @Texto)";

                    string tipoReferencia = radioButton1.Checked ? "ID" : "E-mail";

                   
                    using (SqlCommand cmdHist1 = new SqlCommand(queryHist, conexao))
                    {
                        cmdHist1.Parameters.AddWithValue("@UsuarioId", Conta.Id);
                        cmdHist1.Parameters.AddWithValue("@Texto", $"{dataHoraStr} Transf. Enviada para o {tipoReferencia} {buscaDestinatario}: -{valorTransferencia:C}");
                        cmdHist1.ExecuteNonQuery();
                    }

                    
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

           
            Conta.Saldo -= valorTransferencia;
            string tipoRefLocal = radioButton1.Checked ? "ID" : "E-mail";
            Conta.Historico.Add($"[{DateTime.Now:dd/MM/yyyy HH:mm}] Transf. Enviada para o {tipoRefLocal} {buscaDestinatario}: -{valorTransferencia:C}");

            
            textBox1.Clear();
            textBox2.Clear();
            MessageBox.Show($"Transferência de {valorTransferencia:C} enviada com sucesso para {destinatarioNome}!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            
            menu_principal main = (menu_principal)Application.OpenForms["menu_principal"];
            if (main != null)
            {
                main.BuscarDadosDoBanco();
                main.AtualizarInterfaceCompleta();
            }
        }

       
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
                                 
        }
    }
}