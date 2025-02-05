using PARKING.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PARKING.DTO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PARKING.GUI
{
    public partial class GUI_CARD : Form
    {
        public GUI_CARD()
        {
            InitializeComponent();
        }

        private DAL_CARD DAL_CARD = new DAL_CARD();
        private DAL_USER DAL_USER = new DAL_USER();

        private void LoadData()
        {
            dataGridView2.DataSource = DAL_CARD.GetCard();
            dataGridView2.DefaultCellStyle.BackColor = Color.White;
            dataGridView2.DefaultCellStyle.ForeColor = Color.FromArgb(34, 36, 39);
            dataGridView2.DefaultCellStyle.SelectionBackColor = Color.Firebrick;
            dataGridView2.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView2.Columns["UName"].HeaderText = "User";
        }
        private void GUI_CARD_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            string keyword = textBox1.Text.Trim();
            DataTable dt = DAL_CARD.SearchByName(keyword);
            if (dt != null)
            {
                dataGridView2.DataSource = dt;
            }
            else { MessageBox.Show("Not found !"); }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Card card = new Card
            {
                ID = textBox2.Text,
                UID = int.Parse(textBox3.Text),
                Vehicle = textBox4.Text,
                Money = int.Parse(textBox5.Text)
            };

            if (DAL_CARD.AddCard(card))
            {
                MessageBox.Show("Add card successful !");
            }
            else { MessageBox.Show("Add card failed !"); }

            LoadData();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            string ID = textBox2.Text;
            if (DAL_CARD.DeleteCard(ID))
            {
                MessageBox.Show("Delete card successful !");
                LoadData();
            }
            else
            {
                MessageBox.Show("Delete card failed");
            }
            LoadData();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            Card card = new Card
            {
                ID = textBox2.Text,
                UID = int.Parse(textBox3.Text),
                Vehicle = textBox4.Text,
                Money = int.Parse(textBox5.Text)
            };

            if (DAL_CARD.UpdateCard(card)) 
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

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dataGridView2.Rows[e.RowIndex];
                    textBox2.Text = row.Cells["ID"].Value.ToString();
                    textBox3.Text = row.Cells["UID"].Value.ToString();
                    textBox4.Text = row.Cells["Vehicle"].Value.ToString();
                    textBox5.Text = row.Cells["Money"].Value.ToString();
                    textBox6.Text = row.Cells["UName"].Value.ToString();
                }
            }
            catch { }
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView2.Columns[e.ColumnIndex].Name == "Money" && e.Value != null)
            {
                // Thêm ký tự "VNĐ" sau số tiền
                e.Value = $"{e.Value:N0} VNĐ";
                e.FormattingApplied = true; // Đảm bảo định dạng đã được áp dụng
            }
        }
    }
}
