using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; 

namespace Horazon_Bank__projetoFinal
{
    public partial class documentos : Form
    {
        private string emailUsuario = "";
        private string nomeUsuario = "";

        public documentos()
        {
            InitializeComponent();
        }

        public documentos(string email, string nome)
        {
            InitializeComponent();
            emailUsuario = email;
            nomeUsuario = nome;
        }

       
        private bool ValidarCartaoCidadao(string cartao)
        {
            if (string.IsNullOrWhiteSpace(cartao))
                return false;

            string cartaoLimpo = cartao.Replace(" ", "").Trim().ToUpper();

            if (!Regex.IsMatch(cartaoLimpo, @"^\d{8}[A-Z]{2}$"))
                return false;

            string parteNumerica = cartaoLimpo.Substring(0, 8);

            if (ContemSequenciaObvia(parteNumerica))
                return false;

            return true;
        }

       
        private bool ValidarPassaporte(string passaporte)
        {
            if (string.IsNullOrWhiteSpace(passaporte))
                return false;

            string passaporteLimpo = passaporte.Replace(" ", "").Trim().ToUpper();

            return Regex.IsMatch(passaporteLimpo, @"^[A-Z]{1,2}\d{6,7}$");
        }

      
        private bool ValidarNIF(string nif)
        {
            if (string.IsNullOrWhiteSpace(nif))
                return false;

            string nifLimpo = nif.Trim();

            if (!Regex.IsMatch(nifLimpo, @"^\d{9}$"))
                return false;

            char primeiroDigito = nifLimpo[0];
            bool prefixoValido = primeiroDigito == '1' || primeiroDigito == '2' ||
                                 primeiroDigito == '5' || primeiroDigito == '6' ||
                                 primeiroDigito == '8' || primeiroDigito == '9';

            if (!prefixoValido)
                return false;

            if (ContemSequenciaObvia(nifLimpo))
                return false;

            return true;
        }

     
        private bool ContemSequenciaObvia(string digitos)
        {
            if (string.IsNullOrWhiteSpace(digitos) || digitos.Length < 3)
                return false;

            bool sequenciaCrescente = true;
            for (int i = 0; i < digitos.Length - 1; i++)
            {
                int atual = digitos[i] - '0';
                int proximo = digitos[i + 1] - '0';

                if (proximo != atual + 1)
                {
                    sequenciaCrescente = false;
                    break;
                }
            }

            if (sequenciaCrescente)
                return true;

            bool sequenciaDecrescente = true;
            for (int i = 0; i < digitos.Length - 1; i++)
            {
                int atual = digitos[i] - '0';
                int proximo = digitos[i + 1] - '0';

                if (proximo != atual - 1)
                {
                    sequenciaDecrescente = false;
                    break;
                }
            }

            if (sequenciaDecrescente)
                return true;

            return digitos.All(c => c == digitos[0]);
        }

        
        private bool ValidarMorada(string morada)
        {
            if (string.IsNullOrWhiteSpace(morada))
                return false;

            return morada.Trim().Length >= 10;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string cartao = textBox1.Text.Trim();
            string nif = textBox2.Text.Trim();
            string morada = textBox3.Text.Trim();

          
            if (string.IsNullOrWhiteSpace(cartao))
            {
                MessageBox.Show("Cartão de Cidadão ou Passaporte é obrigatório.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }

            bool cartaoValido = ValidarCartaoCidadao(cartao);
            bool passaporteValido = ValidarPassaporte(cartao);

            if (!cartaoValido && !passaporteValido)
            {
                MessageBox.Show(
                    "Cartão de Cidadão ou Passaporte inválido.\n\n" +
                    "Cartão de Cidadão: 12345678 AB (sem sequências óbvias)\n" +
                    "Passaporte: AB123456",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }

           
            if (!ValidarNIF(nif))
            {
                MessageBox.Show(
                    "NIF inválido.\n\n" +
                    "O NIF deve ter 9 dígitos,\n" +
                    "e não pode ser uma sequência óbvia.\n" +
                    "Exemplo: 245678912",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Focus();
                return;
            }

           
            if (!ValidarMorada(morada))
            {
                MessageBox.Show(
                    "Morada inválida.\n\n" +
                    "A morada deve ter pelo menos 10 caracteres.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Focus();
                return;
            }

            
            string documentoFormatado = cartao.Replace(" ", "").ToUpper();

          
            Conta.CartaoCidadaoPassaporte = documentoFormatado;
            Conta.NIF = nif;
            Conta.Morada = morada;

           
            string emailAlvo = !string.IsNullOrEmpty(Conta.Email) ? Conta.Email : emailUsuario;

            if (string.IsNullOrEmpty(emailAlvo))
            {
                MessageBox.Show("Erro do Sistema: Identificação do utilizador (E-mail) não encontrada.", "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection conexao = Database.GetConnection())
            {
                try
                {
                    conexao.Open(); 

                    string queryUpdate = @"UPDATE Utilizadores 
                                   SET CartaoCidadao = @Cartao, 
                                       NIF = @NIF, 
                                       Morada = @Morada 
                                   WHERE Email = @Email";

                    using (SqlCommand cmd = new SqlCommand(queryUpdate, conexao))
                    {
                        cmd.Parameters.AddWithValue("@Cartao", documentoFormatado);
                        cmd.Parameters.AddWithValue("@NIF", nif);
                        cmd.Parameters.AddWithValue("@Morada", morada);
                        cmd.Parameters.AddWithValue("@Email", emailAlvo);

                        int linhasAfetadas = cmd.ExecuteNonQuery();

                        
                        if (linhasAfetadas == 0)
                        {
                            
                            MessageBox.Show("Aviso: O registo base do utilizador ainda não existe no SQL Server. Os dados foram guardados na memória temporária.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro de Conexão com o SQL Server: " + ex.Message, "Erro SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; 
                }
            }

            
            this.Hide();
            using (var Loading = new Loading())
            {
                Loading.ShowDialog();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void documentos_Load(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                int cursorPos = textBox1.SelectionStart;
                textBox1.Text = textBox1.Text.ToUpper();
                textBox1.SelectionStart = cursorPos;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string text = textBox2.Text;
            string cleaned = Regex.Replace(text, @"[^\d]", "");

            if (cleaned != text)
            {
                textBox2.Text = cleaned;
                textBox2.SelectionStart = cleaned.Length;
            }

            if (textBox2.Text.Length > 9)
            {
                textBox2.Text = textBox2.Text.Substring(0, 9);
                textBox2.SelectionStart = 9;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }
    }
}