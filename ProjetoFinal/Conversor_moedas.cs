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
    public partial class Conversor_moedas : Form
    {
        public Conversor_moedas()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (guna2ComboBox3.Text == "" || textBox1.Text == "")
            {
                MessageBox.Show("Tem de preencher todos os campos!",
                                "Aviso",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            decimal euros;

            if (!decimal.TryParse(textBox1.Text, out euros))
            {
                MessageBox.Show("Introduza apenas números.",
                                "Erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            if (euros < 0)
            {
                MessageBox.Show("Não são permitidos valores negativos.",
                                "Aviso",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            decimal taxa = 0;
            string simbolo = "";

            switch (guna2ComboBox3.Text)
            {
                case "Dólar Americano (USD)":
                    taxa = 1.15m;
                    simbolo = "USD";
                    break;

                case "Libra Esterlina (GBP)":
                    taxa = 0.86m;
                    simbolo = "GBP";
                    break;

                case "Iene Japonês (JPY)":
                    taxa = 166.50m;
                    simbolo = "JPY";
                    break;

                case "Franco Suíço (CHF)":
                    taxa = 0.94m;
                    simbolo = "CHF";
                    break;

                case "Dólar Canadense (CAD)":
                    taxa = 1.56m;
                    simbolo = "CAD";
                    break;

                case "Dólar Australiano (AUD)":
                    taxa = 1.76m;
                    simbolo = "AUD";
                    break;

                case "Yuan Renminbi Chinês (CNY)":
                    taxa = 8.30m;
                    simbolo = "CNY";
                    break;

                case "Rupia Indiana (INR)":
                    taxa = 99.20m;
                    simbolo = "INR";
                    break;

                case "Real Brasileiro (BRL)":
                    taxa = 6.35m;
                    simbolo = "BRL";
                    break;

                case "Peso Mexicano (MXN)":
                    taxa = 21.90m;
                    simbolo = "MXN";
                    break;

                case "Rublo Russo (RUB)":
                    taxa = 92.00m;
                    simbolo = "RUB";
                    break;

                case "Won Sul-Coreano (KRW)":
                    taxa = 1570.00m;
                    simbolo = "KRW";
                    break;

                case "Rand Sul-Africano (ZAR)":
                    taxa = 20.50m;
                    simbolo = "ZAR";
                    break;

                case "Coroa Sueca (SEK)":
                    taxa = 11.00m;
                    simbolo = "SEK";
                    break;

                case "Coroa Norueguesa (NOK)":
                    taxa = 11.70m;
                    simbolo = "NOK";
                    break;

                case "Coroa Dinamarquesa (DKK)":
                    taxa = 7.46m;
                    simbolo = "DKK";
                    break;

                case "Dólar de Singapura (SGD)":
                    taxa = 1.48m;
                    simbolo = "SGD";
                    break;

                case "Dólar Neozelandês (NZD)":
                    taxa = 1.91m;
                    simbolo = "NZD";
                    break;

                case "Lira Turca (TRY)":
                    taxa = 45.50m;
                    simbolo = "TRY";
                    break;

                case "Baht Tailandês (THB)":
                    taxa = 42.30m;
                    simbolo = "THB";
                    break;

                case "Rupia Indonésia (IDR)":
                    taxa = 18900m;
                    simbolo = "IDR";
                    break;

                case "Ringgit Malaio (MYR)":
                    taxa = 4.90m;
                    simbolo = "MYR";
                    break;

                case "Peso Filipino (PHP)":
                    taxa = 65.00m;
                    simbolo = "PHP";
                    break;

                case "Dong Vietnamita (VND)":
                    taxa = 30000m;
                    simbolo = "VND";
                    break;

                case "Riyal Saudita (SAR)":
                    taxa = 4.31m;
                    simbolo = "SAR";
                    break;

                case "Shekel Israelense (ILS)":
                    taxa = 4.00m;
                    simbolo = "ILS";
                    break;

                case "Peso Chileno (CLP)":
                    taxa = 1080m;
                    simbolo = "CLP";
                    break;

                case "Peso Colombiano (COP)":
                    taxa = 4700m;
                    simbolo = "COP";
                    break;

                case "Sol Peruano (PEN)":
                    taxa = 4.20m;
                    simbolo = "PEN";
                    break;
            }

            decimal resultado = euros * taxa;

            label4.Text = $"Valor da conversão e de: {euros:N2} EUR = {resultado:N2} {simbolo}";
        }
          
        

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();         
            guna2ComboBox3.SelectedIndex = -1; 
            label4.Text = "Valor da conversão e de: ";          

            textBox1.Focus();          
        }

        private void Conversor_moedas_Load(object sender, EventArgs e)
        {

        }
    }
}
