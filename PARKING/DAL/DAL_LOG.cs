using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PARKING.DTO;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace PARKING.DAL
{
    internal class DAL_LOG : DBConnect
    {
        public DataTable GetCardInfor(string ID)
        {
            string query = @"select c.ID, c.Vehicle, l.CheckIn, l.CheckOut, c.Money
                             from [PARKING].[dbo].[Card] as c
                             left join [PARKING].[dbo].[CardLog] as l 
                             on c.ID = l.CardID
                             where c.ID = @ID";


            try
            {
                Connect();
                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                    cmd.Parameters.AddWithValue("@ID", ID);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { Disconnect(); }
        }



        //     Thêm log check-in hoặc check-out vào bảng CardLog
        public void UpdateCardLog(string cardID, DateTime checkInTime)
        {
            string query = @"IF NOT EXISTS (select 1 from [PARKING].[dbo].[CardLog] where CardID = @CardID and CheckOut is null)
                                 BEGIN
                                    insert into [PARKING].[dbo].[CardLog] (CardID, CheckIn, LogDate)
                                    values (@CardID, @CheckInTime, GETDATE())
                                 END
                             ELSE
                                 BEGIN
                                    update [PARKING].[dbo].[CardLog]
                                    set CheckOut = @CheckOutTime
                                    where CardID = @CardID and CheckOut is NULL
                                 END";

            try
            {
                Connect();
                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                    cmd.Parameters.AddWithValue("@CardID", cardID);
                    cmd.Parameters.AddWithValue("@CheckInTime", checkInTime);
                    cmd.Parameters.AddWithValue("@CheckOutTime", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            finally { Disconnect(); }

        }

        public DataTable GetLogsForDate(DateTime selectedDate)
        {
            string query = @"select c.ID, u.UName, c.Vehicle,
                                    convert(VARCHAR(5), l.CheckIn, 108) AS CheckIn, 
                                    convert(VARCHAR(5), l.CheckOut, 108) AS CheckOut, -- Chỉ lấy giờ và phút
                                    c.Money
                                    from [PARKING].[dbo].[CardLog] as l
                                    join [PARKING].[dbo].[Card] as c on l.CardID = c.ID
                                    left join [PARKING].[dbo].[CardUser] as u on c.UID = u.ID
                             where CAST(l.LogDate as DATE) = @SelectedDate";

            try
            {
                Connect();
                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                    cmd.Parameters.AddWithValue("@SelectedDate", selectedDate.Date);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable logs = new DataTable();
                        adapter.Fill(logs);
                        return logs;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                Disconnect();
            }
        }

        public DataTable GetAllLogsUpToDate()
        {
            string query = @"select c.ID, c.Vehicle,
                                    convert(VARCHAR(5), l.CheckIn, 108) as CheckIn, 
                                    convert(VARCHAR(5), l.CheckOut, 108) as CheckOut, -- Chỉ lấy giờ và phút
                                    c.Money
                             from [PARKING].[dbo].[CardLog] as l
                             join [PARKING].[dbo].[Card] as c   
                             on l.CardID = c.ID
                             where CAST(l.LogDate as DATE) <= CAST(GETDATE() as DATE)";

            try
            {
                Connect();
                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                   
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable logs = new DataTable();
                        adapter.Fill(logs);
                        return logs;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                Disconnect();
            }
        }


        public DataRow GetCardLatestLog(string cardID)
        {
            string query = @"select TOP 1 *
                             from [PARKING].[dbo].[CardLog]
                             where CardID = @CardID
                             order by LogDate DESC, CheckIn DESC";

            try
            {
                Connect();
                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                    cmd.Parameters.AddWithValue("@CardID", cardID);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        if (dataTable.Rows.Count > 0)
                        {
                            return dataTable.Rows[0];
                        }
                        else
                        {
                            return null; // Không tìm thấy log gần nhất
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                Disconnect();
            }
        }

        public void UpdateCardBalance(string cardID, double amount)
        {
            string query = @"update [PARKING].[dbo].[Card] set Money = Money - @Amount where ID = @CardID";

            try
            {


                Connect();
                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {

                    cmd.Parameters.AddWithValue("@CardID", cardID);
                    cmd.Parameters.AddWithValue("@Amount", amount);


                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return; // Nếu có lỗi, coi như không có thẻ nào chưa check-out
            }
            finally
            {
                Disconnect();
            }
        }

    }
}
