using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PARKING.DTO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace PARKING.DAL
{
    public class DAL_CARD : DBConnect
    {
        public DataTable GetCard()
        {
            string query = @"select u.UName, c.ID, c.Vehicle, c.Money, c.UID
                            from [PARKING].[dbo].[Card] as c
                            inner join [PARKING].[dbo].[CardUser] as u on c.UID = u.ID ";
            return GetDataTable(query);
        }


        public bool AddCard(Card card)
        {
            string query = @"insert into [PARKING].[dbo].[Card] (ID, UID, Vehicle, Money)
                             values (@ID, @UID, @Vehicle, @Money)";
            try
            {
                Connect();
                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                    cmd.Parameters.AddWithValue("@ID", card.ID);
                    cmd.Parameters.AddWithValue("@UID", card.UID);
                    cmd.Parameters.AddWithValue("@Vehicle", card.Vehicle);
                    cmd.Parameters.AddWithValue("@Money", card.Money);

                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally { Disconnect(); }
        }

        public bool DeleteCard(string ID)
        {
            string query = "delete from [PARKING].[dbo].[Card] where ID = @ID";

            try
            {
                Connect();
                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                    cmd.Parameters.AddWithValue("@ID", ID);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally { Disconnect(); }
        }

        public bool UpdateCard(Card card)
        {
            string query = @"update [PARKING].[dbo].[Card]
                             set ID = @ID, UID = @UID, Vehicle = @Vehicle, Money = @Money
                             where UID = @UID";

            try
            {
                Connect();
                using (SqlCommand command = new SqlCommand(query, sqlCon))
                {
                    command.Parameters.AddWithValue("@ID", card.ID);
                    command.Parameters.AddWithValue("@UID", card.UID);
                    command.Parameters.AddWithValue("@Vehicle", card.Vehicle);
                    command.Parameters.AddWithValue("@Money", card.Money);

                    command.ExecuteNonQuery();
                    return true;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            finally { Disconnect(); }
        }

        public DataTable SearchByName(string name)
        {
            string query = @"select u.UName, c.ID, c.Vehicle, c.Money, c.UID 
                             from [PARKING].[dbo].[Card] as c
                             join [PARKING].[dbo].[CardUser] as u
                             on c.UID = u.ID
                             where u.UName like @UName"
            ;

            try
            {
                Connect();
                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                    // Gán tham số @UName với giá trị tìm kiếm
                    cmd.Parameters.AddWithValue("@UName", "%" + name + "%");

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt); 
                        return dt;
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

       

    }
}
