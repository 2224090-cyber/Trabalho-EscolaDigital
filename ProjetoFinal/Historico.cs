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
    public partial class Historico : Form
    {
        public Historico()
        {
            InitializeComponent();
            Conta.ValoresAlterados += AtualizarHistorico;
        }


        private void Historico_VisibleChanged(object sender, EventArgs e)
        {
            AtualizarHistorico();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            AtualizarHistorico();
        }

        private void AtualizarHistorico()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(AtualizarHistorico));
                return;
            }

            StringBuilder sb = new StringBuilder();

            for (int i = Conta.Historico.Count - 1; i >= 0; i--)
            {
                sb.AppendLine(Conta.Historico[i]);
            }

            label2.Text = sb.ToString();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }

        private void label2_Click(object sender, EventArgs e)
        {
       


        }

        private void button1_Click(object sender, EventArgs e)
        {

            Conta.LimparHistorico();

        }
    }
}
