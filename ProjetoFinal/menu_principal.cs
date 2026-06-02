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
        public menu_principal()
        {
            InitializeComponent();
        }
        bool menuExpand = false;

        private void menu_principal_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void menuTransition_Tick(object sender, EventArgs e)
        {
            if(menuExpand == false )
            {
                menuContainer.Height += 10;
                if(menuContainer.Height >= 242)
                {
                    menuTransition.Stop();
                    menuExpand = true;
                    
                }
                else
                {
                    menuContainer.Height -= 10;
                    if (menuContainer.Height <= 137)
                    {
                        menuTransition.Stop();
                        menuExpand = false;
                    }

                }

            }
            }

        private void Config_Click(object sender, EventArgs e)
        {

            menuTransition.Start();


        }
    }
}
