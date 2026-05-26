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

        private void progressBar1_Click(object sender, EventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Step = 1;
            progressBar1.PerformStep();
            progressBar1.Value = 0;
            for (int i = 0; i <= 100; i++)
            {
                Thread.Sleep(50);
                this.Invoke(new Action(() => progressBar1.Value = i));
            }
            menu_principal menu_Principal = new menu_principal ();
            menu_Principal.WindowState = this.WindowState;
            menu_Principal.Size = this.Size;
            menu_Principal.StartPosition = FormStartPosition.Manual;
            menu_Principal.Location = this.Location;

            this.Hide();
            menu_Principal.Show();
        }
    }
}
