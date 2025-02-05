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
using System.IO.Ports;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PARKING.GUI
{
    public partial class GUI_DASHBOARD : Form
    {
       
        private DAL_LOG DAL_LOG = new DAL_LOG();
        //private SerialPort serCOM = new SerialPort();

        public GUI_DASHBOARD()
        {
            InitializeComponent();
            string[] Baudrate = { "300", "600", "1200", "2400", "4800", "9600", "14400", "19200" };
            cboBaudrate.Items.AddRange(Baudrate);
            Control.CheckForIllegalCrossThreadCalls = false;
          
            label8.Text = Program.slot.ToString();

            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.ForeColor = Color.FromArgb(34, 36, 39);
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Firebrick;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
        }
        


        private void GUI_DASHBOARD_Load(object sender, EventArgs e)
        {
            cboBaudrate.Text = string.Empty; 
            cboCOM.DataSource = SerialPort.GetPortNames();
            cboBaudrate.Text = "9600";

        }

       

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Money" && e.Value != null)
            {
                // Thêm ký tự "VNĐ" sau số tiền
                e.Value = $"{e.Value:N0} VNĐ";
                e.FormattingApplied = true; // Đảm bảo định dạng đã được áp dụng
            }
        }

        private void LoadData()
        {
            DateTime selectedDate = dateTimePicker1.Value;
            dataGridView1.DataSource = DAL_LOG.GetLogsForDate(selectedDate);
            UpdateStatistics();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void UpdateStatistics()
        {
            // Lấy tất cả log từ trước đến ngày hôm nay
            DataTable allLogs = DAL_LOG.GetAllLogsUpToDate();

            // Đếm số thẻ chưa check-out
            int occupiedSlots = allLogs.AsEnumerable()
                                       .Count(row => row["CheckOut"] == DBNull.Value);

            // Tính số slot khả dụng
            int availableSlots = Program.slot - occupiedSlots;
            label8.Text = $"{availableSlots}"; 
            


            // Đếm số lượng thẻ chưa check-out theo loại xe
            var vehicleCounts = allLogs.AsEnumerable()
                                       .Where(row => row["CheckOut"] == DBNull.Value) // Chỉ thẻ chưa check-out
                                       .GroupBy(row => row["Vehicle"]) // Nhóm theo loại xe
                                       .Select(group => new
                                       {
                                           Vehicle = group.Key.ToString(),
                                           Count = group.Count()
                                       })
                                       .ToList();

            
            int carCount = 0;
            int bikeCount = 0;

            foreach (var vehicle in vehicleCounts)
            {
                if (vehicle.Vehicle == "Car")
                {
                    carCount = vehicle.Count;
                }
                else if (vehicle.Vehicle == "Bike")
                {
                    bikeCount = vehicle.Count;
                }
            }

 
            label6.Text = $"{carCount}"; 
            label7.Text = $"{bikeCount}"; 
        }

        public double CalculateFee(string vehicleType, DateTime checkIn, DateTime checkOut)
        {
            double fee = 0;
            TimeSpan timeDifference = checkOut - checkIn;

            
            if (timeDifference.Days > 0)
            {
                int days = timeDifference.Days; 
                if (vehicleType == "Bike")
                {
                    fee += days * 10000; 
                }
                else if (vehicleType == "Car")
                {
                    fee += days * 50000; 
                }
            }

            // fee in day
            DateTime sameDayCheckOut = checkOut.Date;
            if (timeDifference.Days == 0 || timeDifference.Hours > 0)
            {
                if (checkOut.Hour >= 18) // Sau 18h
                {
                    fee += vehicleType == "Bike" ? 4000 : 20000;
                }
                else // Trước 18h
                {
                    fee += vehicleType == "Bike" ? 2000 : 10000;
                }
            }

            return fee;
        }

        private void butConnect_Click(object sender, EventArgs e)
        {
            serCOM.PortName = cboCOM.Text;
            serCOM.BaudRate = Convert.ToInt32(cboBaudrate.Text);
            
            serCOM.Open();

            if (serCOM.IsOpen) { MessageBox.Show("Connect successful !"); }
            else MessageBox.Show("Connect failed !");
            LoadData();
            //serCOM.WriteLine($"AVAILABLESLOTS{int.Parse(label8.Text)}");
        }

        private void butDisconnect_Click(object sender, EventArgs e)
        {
            serCOM.Close();
            if (!serCOM.IsOpen) MessageBox.Show("Disconnect !");
            else MessageBox.Show("Disconnect failed !");
        }

        private void serCOM_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string ID = serCOM.ReadLine().Trim();
            
            this.Invoke(new Action(() =>
            {

                DataTable CardInfor = DAL_LOG.GetCardInfor(ID);
                if (CardInfor.Rows.Count > 0)
                {
                    string Vehicle = CardInfor.Rows[0]["Vehicle"].ToString();
                    double money = Convert.ToDouble(CardInfor.Rows[0]["Money"]);
                    DataRow latestLog = DAL_LOG.GetCardLatestLog(ID);

                    if (latestLog != null)
                    {

                        
                        var checkOutValue = latestLog["CheckOut"]; 
                        DateTime checkIn = latestLog["CheckIn"] != DBNull.Value
                                                                ? Convert.ToDateTime(latestLog["CheckIn"])
                                                                : DateTime.MinValue;
                        DateTime now = DateTime.Now;
                        double fee = CalculateFee(Vehicle, checkIn, now);
                        if (checkOutValue == DBNull.Value)
                        {
                            //DateTime now = DateTime.Now;
                            DAL_LOG.UpdateCardLog(ID, now);
                            //double fee = CalculateFee(Vehicle, checkIn, now);

                            while (money < fee)
                            {
                                MessageBox.Show("Insufficient balance! Please recharge your card.");
                                // Thoát khỏi vòng lặp để người dùng quẹt thẻ lại
                                return; // Kết thúc xử lý và chờ người dùng quẹt lại thẻ
                            }

                            // Nếu đủ tiền, thực hiện check-out
                            DAL_LOG.UpdateCardBalance(ID, fee);
                            MessageBox.Show("Check-out successful!");
                            serCOM.WriteLine("GOODBYE"); // Gửi tín hiệu mở cổng
                            LoadData(); // Cập nhật giao diện
                        }
                        else if (int.Parse(label8.Text) == 0)
                        {
                            MessageBox.Show("Full slot!");
                        }
                        else
                        {
                            //DateTime now = DateTime.Now;

                            while (money < fee) // Giả sử có mức phí tối thiểu để vào bãi
                            {
                                MessageBox.Show("Insufficient balance! Please recharge your card.");
                                // Thoát khỏi vòng lặp để người dùng quẹt thẻ lại
                                return; // Kết thúc xử lý và chờ người dùng quẹt lại thẻ
                            }

                            // Nếu đủ tiền, thực hiện check-in
                            DAL_LOG.UpdateCardLog(ID, now);
                            MessageBox.Show("Check-in successful!");
                            serCOM.WriteLine("WELCOME"); // Gửi tín hiệu mở cổng
                            LoadData(); // Cập nhật giao diện
                        }

                    }

                }
                else
                {
                    // Trường hợp thẻ không hợp lệ
                    MessageBox.Show("Invalid card!");
                    serCOM.WriteLine("DENY"); // Gửi tín hiệu từ chối
                }


            }));
        }

       
    }    
}


