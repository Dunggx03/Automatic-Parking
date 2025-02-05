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
    public partial class GUI_USER : Form
    {
        public GUI_USER()
        {
            InitializeComponent();
        }

        private DAL_USER DAL_USER = new DAL_USER();

        private void GUI_USER_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadCbo();
        }
        private void LoadData()
        {


            DataTable dt = DAL_USER.GetUser();
            dataGridView1.DataSource = dt;
          
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.ForeColor = Color.FromArgb(34, 36, 39);
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Firebrick;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;



        }
        private void LoadCbo()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("admin");
            comboBox1.Items.Add("personal");
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }




        private void buttonAdd_Click(object sender, EventArgs e)
        {
            CardUser user = new CardUser
            {
                UName = textBox3.Text,
                Email = textBox4.Text,
                Pass = textBox5.Text,
                Access = comboBox1.Text
            };

            if (DAL_USER.AddUser(user))
            {
                MessageBox.Show("Add user successful !");
                LoadData();
            }
            else { MessageBox.Show("Add user failed !"); }

            LoadData();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int ID = int.Parse(textBox2.Text);
            if (DAL_USER.DeleteUser(ID))
            {
                MessageBox.Show("Delete user successful !");
                LoadData();
            }
            else
            {
                MessageBox.Show("Delete user failed");
            }
            LoadData() ;
        }

        private void buttonSreach_Click(object sender, EventArgs e)
        {
            string keyword = textBox1.Text.Trim();
            DataTable dt = DAL_USER.SearchByName(keyword);
            if (dt != null)
            {
                dataGridView1.DataSource = dt;
            }
            else { MessageBox.Show("Not found !"); }
            

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    textBox2.Text = row.Cells["ID"].Value.ToString();
                    textBox3.Text = row.Cells["UName"].Value.ToString();
                    textBox4.Text = row.Cells["Email"].Value.ToString();
                    textBox5.Text = row.Cells["Pass"].Value.ToString();
                    comboBox1.Text = row.Cells["Access"].Value.ToString();
                }
            }
            catch { }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            CardUser user = new CardUser
            {
                ID = int.Parse(textBox2.Text),
                UName = textBox3.Text,
                Email = textBox4.Text,
                Pass = textBox5.Text,
                Access = comboBox1.Text,
            };
            if (DAL_USER.UpdateUser(user))
            {
                MessageBox.Show("Update successful !");
                LoadData();
            }
            else
            {
                MessageBox.Show("Update failed");
            }
            LoadData();
        }
    }
}
