using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PARKING.DAL;
using PARKING.DTO;

namespace PARKING.GUI
{
    public partial class GUI_HOME : Form
    {
        private Form currentFormChild;
        private void OpenChildForm(Form childForm)
        {
            if (currentFormChild != null)
            {
                currentFormChild.Close();
            }
            currentFormChild = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panel3.Controls.Add(childForm);
            panel3.Controls.SetChildIndex(childForm, 0);
            panel3.BringToFront();
            childForm.Show();


        }

        public GUI_HOME()
        {
            InitializeComponent();
            label2.Text = Program.UName;

            OpenChildForm (new GUI_DASHBOARD());
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenChildForm(new GUI_USER());
            label3.Text = "USER";
            panel6.Height = button2.Height;
            panel6.Top = button2.Top;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenChildForm(new GUI_CARD());
            label3.Text = "CARD";
            panel6.Height = button3.Height;
            panel6.Top = button3.Top;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel6.Height = button4.Height;
            panel6.Top = button4.Top;
            if (Program.UName != null)
            {
                this.Hide();
                MessageBox.Show("Log out successful !");
                GUI_LOGIN f = new GUI_LOGIN();
                f.ShowDialog();
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenChildForm(new GUI_DASHBOARD());
            panel6.Height = button1.Height;
            panel6.Top = button1.Top;
            label3.Text = "HOME";
        }


    }
}
