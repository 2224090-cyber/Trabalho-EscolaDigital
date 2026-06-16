using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Horazon_Bank__projetoFinal
{
    public partial class menu_principal : Form
    {
        private Form frmAtivo;
        public menu_principal()
        {
            InitializeComponent();
        }

        public void AtualizarSaldo()
        {
            label2.Text = $"Saldo: {Conta.Saldo:C}";
        }

        private void AtualizarPoupanca()
        {
            label1.Text = $"Poupança: {Conta.Poupanca:C}";
        }


        private void FormPoupanca_Load(object sender, EventArgs e)
        {
            AtualizarPoupanca();
        }



        private void Menu_Principal_Load(object sender, EventArgs e)
        {
            AtualizarSaldo();
        }

        private void Menu_Principal_Activated(object sender, EventArgs e)
        {
            AtualizarSaldo();
        }

        private void FormShow(Form frm)
        {
            ActiveFormClose();
            frmAtivo = frm;
            frm.TopLevel = false;
            panel3.Controls.Add(frm);
            frm.BringToFront();
            frm.Show();
        }

        private void ActiveFormClose()
        {
            if (frmAtivo != null)
                frmAtivo.Close();
        }

        private void ActiveButton(Button frmAtivo)
        {
            foreach (Control ctrl in panel1.Controls)
                ctrl.ForeColor = Color.White;
            frmAtivo.ForeColor = Color.Red;

        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

       

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ActiveButton(button1);
            ActiveFormClose();

            AtualizarSaldo();
            AtualizarSaldo();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ActiveButton(button2);
            FormShow(new Deposito_Saque());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ActiveButton(button3);
            FormShow(new Trasfere());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ActiveButton(button4);
            FormShow(new Emprestimos());

        }

        private void button5_Click(object sender, EventArgs e)
        {
            ActiveButton(button5);
            FormShow(new Poupanca());

           


        }

        private void button6_Click(object sender, EventArgs e)
        {

                ActiveButton(button6);
                FormShow(new Conversor_moedas());

        }

      
        private void button7_Click(object sender, EventArgs e)
        {

            ActiveButton(button7);
            FormShow(new Historico());

        }

       

        private void button9_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            label2.Text = $"Saldo: {Conta.Saldo:C}";
        }
    }
}
