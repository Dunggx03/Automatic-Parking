using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PARKING.DAL
{
    public class DBConnect
    {
        public string strCon = @"Data Source=*your DB src*; Initial Catalog=PARKING;Integrated Security=True";
        public SqlConnection sqlCon;
        public SqlCommand sqlCom;
        public SqlDataAdapter sqlAdap;
        public DataSet ds;

        public void Connect()
        {
            sqlCon = new SqlConnection(strCon);
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
        }

        public void Disconnect()
        {
            if (sqlCon.State == ConnectionState.Open)
            {
                sqlCon.Close();
            }
        }
        
        public void ThucThiPKN(string strSQL)    // query n display data
        {
            using (SqlDataAdapter sqlAdap = new SqlDataAdapter(strSQL, strCon))
            {
                ds = new DataSet();
                sqlAdap.Fill(ds);
            }
        }

        public DataTable GetDataTable(string strSelect)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter sqlAdap = new SqlDataAdapter(strSelect, strCon))
            {
                ds = new DataSet();
                sqlAdap.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0].Copy();   // cf data not changed
                }
            }
            return dt;
        }
    }
}
