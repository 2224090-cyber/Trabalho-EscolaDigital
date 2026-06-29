using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms; 

namespace Horazon_Bank__projetoFinal
{
   
    public enum ModoVerificacao
    {
        CriarConta,
        ResetSenha
    }

    public class email
    {
        public string Provedor { get; set; }   // ex: "smtp.gmail.com"
        public int Porta { get; set; }          // ex: 587
        public string Username { get; set; }    // email do banco
        public string Password { get; set; }    // senha de app do Gmail

        public email(string provedor, int porta, string username, string password)
        {
            Provedor = provedor;
            Porta = porta;
            Username = username;
            Password = password;
        }

        public bool EnviarCodigoVerificacao(string emailDestino, string codigo)
        {
            try
            {
                MailMessage mensagem = new MailMessage();
                mensagem.From = new MailAddress(Username, "Horazon Bank");
                mensagem.To.Add(emailDestino);
                mensagem.Subject = "Código de Verificação - Horazon Bank";
                mensagem.Body = $"O seu código de verificação é: {codigo}\n\n" +
                                 "Não partilhe este código com ninguém.\n" +
                                 "Se não solicitou esta verificação, ignore este email.";
                mensagem.IsBodyHtml = false;

                using (SmtpClient cliente = new SmtpClient(Provedor, Porta))
                {
                    cliente.Credentials = new NetworkCredential(Username, Password);
                    cliente.EnableSsl = true;
                    cliente.Send(mensagem);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBoxAvisoErro(ex.Message);
                return false;
            }
        }

        private void MessageBoxAvisoErro(string mensagem)
        {
            System.Windows.Forms.MessageBox.Show(
                $"Erro ao enviar email: {mensagem}",
                "Erro de Envio",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error);
        }
    }
}