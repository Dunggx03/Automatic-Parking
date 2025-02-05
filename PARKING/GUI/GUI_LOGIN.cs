using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PARKING.DAL;
using PARKING.DTO;


namespace PARKING.GUI
{
    public partial class GUI_LOGIN : Form
    {
        DAL_USER DAL_USER = new DAL_USER();
        public GUI_LOGIN()
        {
            InitializeComponent();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            string Email = textBox1.Text;
            string Pass = textBox2.Text;
            string UName;
            bool res = DAL_USER.AuthenticateUser(Email, Pass, out UName);
            if (res)
            {
                

                this.Hide();
                Program.UName = UName;
                MessageBox.Show("Login successful !");

                GUI_HOME f = new GUI_HOME();
                f.ShowDialog();
                this.Close();
            }
            else { MessageBox.Show("Login failed")  ; }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Close program ?", "Confirmation", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            else
            {
                this.Close();
            }
        }


    }
}
