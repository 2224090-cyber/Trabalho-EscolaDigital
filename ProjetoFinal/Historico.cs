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

        class Historico1
        {
            public static List<string> Operacoes { get; } = new List<string>();
        }


        public Historico()
        {
            InitializeComponent();
            AtualizarHistorico(); // Adiciona isto aqui!
        }

        private void Historico_Load(object sender, EventArgs e)
        {
            AtualizarHistorico();
        }

        private void AtualizarHistorico()
        {
            label2.Text = "";

            foreach (string operacao in Historico1.Operacoes)
            {
                label2.Text += operacao + Environment.NewLine;
            }
        }

        private void Historico_VisibleChanged(object sender, EventArgs e)
        {
            AtualizarHistorico();
        }

        private void Historico_Shown(object sender, EventArgs e)
        {
            AtualizarHistorico();
        }
        private void label2_Click(object sender, EventArgs e)
        {
            label2.Text = ""; foreach (string operacao in Historico1.Operacoes) { label2.Text += operacao + Environment.NewLine; }




        }
    }

}










