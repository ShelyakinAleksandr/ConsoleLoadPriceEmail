using ConsoleLoadPriceEmail.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;

namespace ConsoleLoadPriceEmail.SqlRequest
{
    class SqlQuery
    {
        public void RecordingPriceDb(List<SuppliersPrice> suppliersPrice)
        {
            try
            {
                Console.WriteLine("Прочитаные записи гружу в базу");

                DataTable TableInsert = new DataTable();

                MySqlConnectionStringBuilder MySqlConStrBld = new MySqlConnectionStringBuilder();

                MySqlConStrBld.Server = "127.0.0.1";
                MySqlConStrBld.UserID = "Admin";
                MySqlConStrBld.Password = "pass";
                MySqlConStrBld.Database = "autoparts";
                MySqlConStrBld.CharacterSet = "utf8";
                MySqlConStrBld.SslMode = MySqlSslMode.Required;



                MySqlConnection con = new MySqlConnection(MySqlConStrBld.ConnectionString);
                // Получаю структуру таблицы из бд
                MySqlDataAdapter adapter = new MySqlDataAdapter(@"select *
                                                                   from priceitems
                                                                   where id = 0", con);

                adapter.Fill(TableInsert);

                adapter.InsertCommand = new MySqlCommand("call AddPrice(@Vendor, @Number, @SearchVendor, @SearchNumber, @Description, @Price, @Count)", con);
                adapter.InsertCommand.CommandType = CommandType.Text;

                adapter.InsertCommand.Parameters.Add(
                    new MySqlParameter("@Vendor", MySqlDbType.VarChar, 64, "Vendor"));

                adapter.InsertCommand.Parameters.Add(
                    new MySqlParameter("@Number", MySqlDbType.VarChar, 64, "Number"));

                adapter.InsertCommand.Parameters.Add(
                    new MySqlParameter("@SearchVendor", MySqlDbType.VarChar, 64, "SearchVendor"));

                adapter.InsertCommand.Parameters.Add(
                    new MySqlParameter("@SearchNumber", MySqlDbType.VarChar, 64, "SearchNumber"));

                adapter.InsertCommand.Parameters.Add(
                    new MySqlParameter("@Description", MySqlDbType.VarChar, 512, "Description"));

                adapter.InsertCommand.Parameters.Add(
                    new MySqlParameter("@Price", MySqlDbType.Decimal, 18, "Price"));
                adapter.InsertCommand.Parameters["@Price"].Precision = 18;
                adapter.InsertCommand.Parameters["@Price"].Precision = 2;

                adapter.InsertCommand.Parameters.Add(
                    new MySqlParameter("@Count", MySqlDbType.Int32, 11, "Count"));

                int counterPosition = 1;
                string nameParametrException = null;

                foreach (SuppliersPrice Price in suppliersPrice)
                {

                    counterPosition++;

                    DataRow dataRowInsert = TableInsert.NewRow();

                    dataRowInsert["Vendor"] = Price.Vendor;
                    dataRowInsert["Number"] = Price.Number;
                    dataRowInsert["SearchVendor"] = Price.SearchVendor;
                    dataRowInsert["SearchNumber"] = Price.SearchNumber;
                    dataRowInsert["Description"] = Price.Description;

                    if (Price.Price != null) dataRowInsert["Price"] = Price.Price;
                    else
                    { 
                        Console.WriteLine("Не смог загрузить позицию " + counterPosition + "; Не Коректное значение цены");
                        continue;
                    }

                    if (Price.Count != null) dataRowInsert["Count"] = Price.Count;
                    else
                    {
                        Console.WriteLine("Не смог загрузить позицию " + counterPosition + "; Не Коректное значение количество");
                        continue;
                    }
                        
                    TableInsert.Rows.Add(dataRowInsert);
                }

                Console.WriteLine("Записываю в базу");

                con.Open();
                adapter.Update(TableInsert);
                con.Close();

                Console.WriteLine("Загрузил " + TableInsert.Rows.Count + " записей.");
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}
