using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Horazon_Bank__projetoFinal
{
    public partial class Loading : Form
    {
        public Loading()
        {
            InitializeComponent();
        }

        private async void progressBar1_Click(object sender, EventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;

            int tempoTotal = 3000;
            int passos = 100;
            int intervalo = tempoTotal / passos;

            for (int i = 0; i <= 100; i++)
            {
                progressBar1.Value = i;
                await Task.Delay(intervalo);
            }

            menu_principal menuPrincipal = new menu_principal();

            menuPrincipal.WindowState = this.WindowState;
            menuPrincipal.Size = this.Size;
            menuPrincipal.StartPosition = FormStartPosition.Manual;
            menuPrincipal.Location = this.Location;

         

            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Loading_Load(object sender, EventArgs e)
        {

        }
    }
}
