using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Server
{
    class Database : Ilogger
    {
        SqlConnection mydb = new SqlConnection("Server=LAPTOP-MR3G567K; Database=Chatroom;Integrated Security=true;");

        public void Save(Message message)
        {
            string thisBody = VerifySQLReady(message.Body);
            try
            {
                mydb.Open();
                string updateQuery = $"INSERT INTO Messages (Sender, Message_Date, Message_Body) VALUES ('{message.UserId}', '{message.messageTime.ToString("yyyy-MM-dd HH:mm:ss tt")}', '{thisBody}')";
                SqlCommand myCmd = new SqlCommand(updateQuery, mydb);
                myCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.Write("Inserting --- " + e);
                Console.ReadLine();
            }
            finally
            {
                mydb.Close();
            }
        }
        public string VerifySQLReady(string text)
        {
            string sqlReady = "";
            sqlReady = text.Replace("'", "''");
            return sqlReady;
        }
    }
}
