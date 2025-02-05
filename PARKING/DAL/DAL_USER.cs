using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using PARKING.DTO;
using System.Collections;



namespace PARKING.DAL
{
    public class DAL_USER : DBConnect
    {
   
        public bool AuthenticateUser(string Email, string Pass, out string UName)
        {
            UName = ""; // Initialize output parameter
            try
            {
                Connect();
                string query = "select UName from [PARKING].[dbo].[CardUser] where Email = @Email and Pass = @Pass and Access = 'admin' " ;

                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                    // Add parameters to avoid SQL Injection
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@Pass", Pass);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            UName = reader["UName"].ToString(); 
                            Program.UName = UName; 
                            return true; 
                        }
                        return false; 
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false; 
            }
            finally
            {
                Disconnect();
            }
        }

        public DataTable GetUser()
        {
            string query = @"select * from [PARKING].[dbo].[CardUser]";
            return GetDataTable(query);
        }

        public bool AddUser(CardUser user)
        {
            string query = @"insert into [PARKING].[dbo].[CardUser] (UName, Email,Pass, Access)
                             values (@UName, @Email, @Pass, @Access)";
            try
            {
                Connect();
                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                    cmd.Parameters.AddWithValue("@UName", user.UName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Pass", user.Pass);
                    cmd.Parameters.AddWithValue("@Access", user.Access);

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
        public bool DeleteUser(int ID)
        {
            string query = "delete from [PARKING].[dbo].[CardUser] where ID = @ID";
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

        public DataTable SearchByName(string name)
        {
            string query = "select * from [PARKING].[dbo].[CardUser] where UName like @UName";
            try
            {
                Connect();
                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                    cmd.Parameters.AddWithValue("@UName", "%" + name + "%");
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
            finally { Disconnect() ; }
        }

        public bool UpdateUser(CardUser user)
        {
            string query = @"update [PARKING].[dbo].[CardUser] 
                             set UName = @UName, Email = @Email, Pass = @Pass, Access = @Access
                             where ID = @ID";
            try
            {
                Connect();
                using (SqlCommand command = new SqlCommand(query, sqlCon))
                {
                    command.Parameters.AddWithValue("@ID", user.ID);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@UName", user.UName);
                    command.Parameters.AddWithValue("@Pass", user.Pass);
                    command.Parameters.AddWithValue("@Access", user.Access);

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
    }
}
