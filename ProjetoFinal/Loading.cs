using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;

using System.Windows.Forms;

namespace Horazon_Bank__projetoFinal
{
    public partial class Loading : Form
    {
        public Loading()
        {
            InitializeComponent();
        }

        // 1. Mudamos a lógica para um método próprio que pode ser chamado automaticamente
        private async Task IniciarCarregamento()
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

            // Cria o menu principal
            menu_principal menuPrincipal = new menu_principal();

            // Copia as dimensões e estados do formulário de Loading
            menuPrincipal.WindowState = this.WindowState;
            menuPrincipal.Size = this.Size;
            menuPrincipal.StartPosition = FormStartPosition.Manual;
            menuPrincipal.Location = this.Location;

            // ✅ CORREÇÃO: Esconde o Loading primeiro
            this.Hide();

            // ✅ CORREÇÃO CRÍTICA: Mostra o Menu Principal no ecrã!
            menuPrincipal.ShowDialog();

            // Fecha o formulário de loading de vez após o menu principal ser fechado
            this.Close();
        }

        // Se o utilizador carregar na barra por engano, também funciona
        private async void progressBar1_Click(object sender, EventArgs e)
        {
            // Apenas executa se a barra ainda não tiver começado (evita loops se clicar várias vezes)
            if (progressBar1.Value == 0)
            {
                await IniciarCarregamento();
            }
        }

        // 2. Acionamos o carregamento automático assim que a tela abre (Load)
        private async void Loading_Load(object sender, EventArgs e)
        {
            // Ativa o carregamento automático sem precisar de cliques!
            await IniciarCarregamento();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }
}