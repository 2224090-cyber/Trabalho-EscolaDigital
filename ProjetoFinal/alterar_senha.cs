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
    public partial class alterar_senha : Form
    {
        public alterar_senha()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        // ===================== BOTÃO 1: VERIFICAR E-MAIL E ENVIAR CÓDIGO =====================
        private void button1_Click(object sender, EventArgs e)
        {
            string emailDigitado = textBox1.Text.Trim();

            // 1. Validação de campo vazio
            if (string.IsNullOrWhiteSpace(emailDigitado))
            {
                MessageBox.Show("Digite o email da sua conta.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }

            // 2. ✅ NOVO: Verificar no SQL Server se o e-mail existe na base de dados
            // Isto garante que funciona mesmo que o utilizador NÃO esteja logado no sistema ainda!
            string query = "SELECT Id, Nome FROM Utilizadores WHERE Email = @Email";
            bool emailExiste = false;

            try
            {
                using (SqlConnection conexao = Database.GetConnection())
                {
                    using (SqlCommand comando = new SqlCommand(query, conexao))
                    {
                        comando.Parameters.AddWithValue("@Email", emailDigitado);
                        conexao.Open();

                        using (SqlDataReader reader = comando.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                emailExiste = true;

                                // Guarda temporariamente na classe global para o próximo formulário saber de quem é a conta
                                Conta.Id = reader["Id"].ToString();
                                Conta.Email = emailDigitado;
                                Conta.Nome = reader["Nome"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao consultar a base de dados: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. Se o e-mail não foi encontrado no banco de dados
            if (!emailExiste)
            {
                MessageBox.Show("Esse email não tem conta criada no Horizon Bank.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }

            // 4. Gerar e enviar o código de verificação
            Conta.CodigoVerificacao = Conta.GerarCodigoVerificacao();

            email emailBanco = new email(
                "smtp.gmail.com",
                587,
                "horizonbank.f1@gmail.com",
                "liog nhuo xddf jpwk" // A tua App Password do Gmail
            );

            // Mostrar um cursor de carregamento (boa prática enquanto envia o email)
            Cursor.Current = Cursors.WaitCursor;
            bool enviado = emailBanco.EnviarCodigoVerificacao(Conta.Email, Conta.CodigoVerificacao);
            Cursor.Current = Cursors.Default;

            if (!enviado)
            {
                MessageBox.Show("Não foi possível enviar o código de verificação. Tente novamente.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Um código de verificação foi enviado para o seu email.", "Código Enviado",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 5. Abrir verificacao_de_conta no modo de reset de senha
            this.Hide();
            using (var verificacao = new verificacao_de_conta(ModoVerificacao.ResetSenha))
            {
                verificacao.ShowDialog();
            }

            this.Close();
        }

        // ===================== BOTÃO 3: VOLTAR AO LOGIN =====================
        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var Form1 = new Form1())
            {
                Form1.ShowDialog();
            }
            this.Close(); // Garante que fecha o formulário antigo da memória
        }
    }
}