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
    public partial class documentos : Form
    {
        public documentos()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {


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
    }
}
