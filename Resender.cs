using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace MCHS_Resender
{
    public static class Resender
    {
        public static List<New_Message> Get(CFG cfg)
        {
            List<New_Message> nm = new List<New_Message>();

            DataTable tableDB = CreateDataTable("TableDB");            
            DataRow rowDB;

            string sqlExpression = "SELECT Id, FoivRequestID FROM Documents WHERE DocType_GUID = 'c54d2ed3-f9df-e711-b918-b4b52f59293c' AND(Status = 'Получен ответ' OR Status = 'Запрос отклонен') AND BeginDate > '20210101'";

            SqlConnection connection = new SqlConnection(cfg.ConnectionString);
            try
            {
                // Открываем подключение
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                using (StreamWriter sw = new StreamWriter(cfg.Path, true, System.Text.Encoding.Default))
                {
                    if (reader.HasRows) // если есть данные
                    {
                        int i = 0;
                        while (reader.Read()) // построчно считываем данные
                        {
                            string Id = reader.GetValue(0).ToString();
                            string FoivRequestID = reader.GetValue(1).ToString();
                            rowDB = tableDB.NewRow();
                            rowDB["Id"] = Id;
                            rowDB["FoivRequestID"] = FoivRequestID;
                            tableDB.Rows.Add(rowDB);
                            //sw.WriteLine(Id); // Использовать для первого заполнения файла CertData.dat, дальше закомментить 
                            i++;
                        }
                    }
                }

                reader.Close();
            }
            catch
            {
               
            }

            for (int i = 0; i < tableDB.Rows.Count; i++)
            {
                bool isAbsent = true;
                using (StreamReader sr = new StreamReader(cfg.Path, System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == tableDB.Rows[i].ItemArray[0].ToString())
                        {
                            isAbsent = false;
                            break;
                        }
                    }
                }
                if (isAbsent)
                {
                    var data = new New_Message
                    {
                        Id = tableDB.Rows[i].ItemArray[0].ToString(),
                        FoivRequestID = tableDB.Rows[i].ItemArray[1].ToString()                        
                    };
                    nm.Add(data);
                }


            }


            return nm;
        }

        internal static void Set(List<New_Message> new_Messages, CFG cfg)
        {
            foreach (var m in new_Messages)
            {
                string sqlExpression = "UPDATE Documents SET StatusChange = 1 WHERE Id = '" + m.Id + "'";

                SqlConnection connection = new SqlConnection(cfg.ConnectionString);
                try
                {
                    // Открываем подключение
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.ExecuteNonQuery();
                    using (StreamWriter sw = new StreamWriter(cfg.Path, true, System.Text.Encoding.Default))
                    {
                        sw.WriteLine(m.Id);
                    }
                }
                catch
                {

                }
            }
        }

        internal static void CopyDataFile(string Path)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(Path);
                File.Copy(di.FullName, di.FullName.Remove(di.FullName.Length - 3, 3) + "_temp.dat", true);
            }
            catch
            {

            }

        }

        private static DataTable CreateDataTable(string v)
        {
            DataTable table = new DataTable(v);
            DataColumn column;
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "Id"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "FoivRequestID"
            };
            table.Columns.Add(column);
            return table;

        }
    }
}
