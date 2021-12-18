using System;
using System.IO;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using MimeKit;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using System.Data;
using MySqlConnector;

namespace ConsoleLoadPriceEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Suppliers> suppliers = new List<Suppliers>();

            suppliers.Add(
                new Suppliers()
                {
                    Id = 1,
                    Name = "ООО доставим в срок",
                    Email = "strannik2292107@yandex.ru",
                    Vendor = "Бренд",
                    Number = "Каталожный номер",
                    Description = "Описание",
                    Price = "Цена, руб.",
                    Count = "Наличие"
                });

            Program Program = new Program();

            Program.LoadPriceEmail(suppliers[0]);
        }

        public void LoadPriceEmail(Suppliers suppliers)
        {
            try
            {
               
                string pathToPrice = null;

                Console.WriteLine("Подключаюсь к почте");

                using (ImapClient client = new ImapClient())
                {
                    client.Connect("imap.yandex.ru", 993, true);
                    client.Authenticate("strannik2292107@yandex.ru", "ktc1vjq1ljv");

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);

                    Console.WriteLine("Подключился");

                    MimeMessage message = null;

                    Console.WriteLine("Ищю последнее сообщение от " + suppliers.Name);
                    var query = SearchQuery.FromContains(suppliers.Email).And(SearchQuery.SubjectContains("Прайс"));

                    List<UniqueId> uid = new List<UniqueId>(inbox.Search(query));

                    Console.WriteLine("Нашел, гружу письмо");

                    message = inbox.GetMessage(uid.Max());

                    Console.WriteLine("Ищю файл с расширением csv");

                    foreach (var attachment in message.Attachments)
                    {
                        if (attachment is MimePart)
                        {
                            var part = (MimePart)attachment;
                            var fileName = part.FileName;

                            using (var stream = File.Create(fileName))
                            {
                                part.Content.DecodeTo(stream);

                                if (".csv" == Path.GetExtension(stream.Name))
                                {
                                    Console.WriteLine("Нашёл");
                                    pathToPrice = stream.Name;
                                }
                            }

                        }
                    }

                    client.Disconnect(true);
                }



                if (pathToPrice != null)
                {
                    List<string> listEror = new List<string>();

                    #region Таблица для выгрузки csv файла

                    DataTable tablePrice = new DataTable();

                    DataColumn dataColumn = new DataColumn();

                    //dataColumn = new DataColumn();
                    //dataColumn.ColumnName = "id";
                    //dataColumn.DataType = Type.GetType("System.Int32");
                    //dataColumn.AutoIncrementStep = 1;
                    //dataColumn.AutoIncrementSeed = 1;
                    //dataColumn.AutoIncrement = true;
                    //tablePrice.Columns.Add(dataColumn);
                    //tablePrice.PrimaryKey = new DataColumn[] { tablePrice.Columns["id"] };

                    dataColumn = new DataColumn();
                    dataColumn.ColumnName = "Vendor";
                    dataColumn.DataType = Type.GetType("System.String");
                    tablePrice.Columns.Add(dataColumn);

                    dataColumn = new DataColumn();
                    dataColumn.ColumnName = "SearchVendor";
                    dataColumn.DataType = Type.GetType("System.String");
                    tablePrice.Columns.Add(dataColumn);

                    dataColumn = new DataColumn();
                    dataColumn.ColumnName = "Number";
                    dataColumn.DataType = Type.GetType("System.String");
                    tablePrice.Columns.Add(dataColumn);

                    dataColumn = new DataColumn();
                    dataColumn.ColumnName = "SearchNumber";
                    dataColumn.DataType = Type.GetType("System.String");
                    tablePrice.Columns.Add(dataColumn);

                    dataColumn = new DataColumn();
                    dataColumn.ColumnName = "Description";
                    dataColumn.DataType = Type.GetType("System.String");
                    tablePrice.Columns.Add(dataColumn);

                    dataColumn = new DataColumn();
                    dataColumn.ColumnName = "Price";
                    dataColumn.DataType = Type.GetType("System.String");
                    tablePrice.Columns.Add(dataColumn);

                    dataColumn = new DataColumn();
                    dataColumn.ColumnName = "Count";
                    dataColumn.DataType = Type.GetType("System.String");
                    tablePrice.Columns.Add(dataColumn);

                    #endregion

                    Console.WriteLine("Скачал, открываю и читаю");

                    using (StreamReader reader = new StreamReader(new FileStream(pathToPrice, FileMode.Open)))
                    {
                        int countRows = 0;

                        var isRecordBad = false;
                        var config = new CsvConfiguration(CultureInfo.CurrentCulture)
                        {
                            Delimiter = ";",
                            BadDataFound = context => { isRecordBad = true; }
                        };

                        using (var csv = new CsvReader(reader, config))
                        {
                            csv.Read();
                            csv.ReadHeader();

                            int nomberRows = 1;

                            while (csv.Read())
                            {
                                nomberRows++;
                            }

                            #region Запись в таблицу
                            //DataRow dataRow = tablePrice.NewRow();

                            //try
                            //{
                            //    dataRow["Vendor"] = csv.GetField<string>(suppliers.Vendor);
                            //    dataRow["SearchVendor"] = new string(csv.GetField<string>(suppliers.Vendor).Where(c => Char.IsLetter(c) || Char.IsDigit(c)).ToArray()).ToUpper();

                            //    dataRow["Number"] = csv.GetField<string>(suppliers.Number);
                            //    dataRow["SearchNumber"] = new string(csv.GetField<string>(suppliers.Number).Where(c => Char.IsLetter(c) || Char.IsDigit(c)).ToArray()).ToUpper();

                            //    if (512 < csv.GetField<string>(suppliers.Description).Length)
                            //        dataRow["Description"] = csv.GetField<string>(suppliers.Description).Substring(0, 512);
                            //    else
                            //        dataRow["Description"] = csv.GetField<string>(suppliers.Description);

                            //    dataRow["Price"] = csv.GetField<double>(suppliers.Price);

                            //    try { dataRow["Count"] = csv.GetField<int>(suppliers.Count); }
                            //    catch (CsvHelper.TypeConversion.TypeConverterException)
                            //    {
                            //        try
                            //        {
                            //            string count = csv.GetField<string>(suppliers.Count);

                            //            if (count.Contains('-'))
                            //                dataRow["Count"] = Convert.ToInt32(count.Substring(count.IndexOf('-') + 1));

                            //            if (count.Contains('>'))
                            //                dataRow["Count"] = Convert.ToInt32(count.Trim('>'));

                            //            if (count.Contains('<'))
                            //                dataRow["Count"] = Convert.ToInt32(count.Trim('<'));
                            //        }
                            //        catch (Exception ex)
                            //        { listEror.Add("строка " + nomberRows); }
                            //    }

                            //    if (isRecordBad)
                            //        listEror.Add("строка " + nomberRows);

                            //    tablePrice.Rows.Add(dataRow);

                            //    isRecordBad = false;
                            //}
                            //catch (Exception) { listEror.Add("строка " + nomberRows); }
                            #endregion

                            Console.WriteLine("Прочитал" + nomberRows); 
                            Console.WriteLine("Записал"+tablePrice.Rows.Count);
                        }
                    }

                    Console.WriteLine("Не смог прочитать " + listEror.Count + " позиций");
                    
                    foreach(string eror in listEror)
                        Console.WriteLine(eror);
                    Console.WriteLine("");

                    Program program = new Program();

                   // program.LoadPriceDb(tablePrice);
                }

            }
            catch (FileNotFoundException) { Console.WriteLine("Ошибка, не смог найти скачаный файл."); }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public void LoadPriceDb(DataTable TablePrice)
        {
            try
            {
                Console.WriteLine("Прочитаные записи гружу в базу");

                DataTable TableInsert = new DataTable();

                MySqlConnectionStringBuilder MySqlConStrBld = new MySqlConnectionStringBuilder();

                MySqlConStrBld.Server = "127.0.0.1";
                MySqlConStrBld.UserID = "Admin";
                MySqlConStrBld.Password = "12345";
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
                    new MySqlParameter( "@Vendor", MySqlDbType.VarChar, 64, "Vendor"));

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

                foreach (DataRow dataRowPrice in TablePrice.Rows)
                {
                    DataRow dataRowInsert = TableInsert.NewRow();

                    dataRowInsert["Vendor"] = dataRowPrice["Vendor"];
                    dataRowInsert["Number"] = dataRowPrice["Number"];
                    dataRowInsert["SearchVendor"] = dataRowPrice["SearchVendor"];
                    dataRowInsert["SearchNumber"] = dataRowPrice["SearchNumber"];
                    dataRowInsert["Description"] = dataRowPrice["Description"];
                    dataRowInsert["Price"] = dataRowPrice["Price"];
                    dataRowInsert["Count"] = dataRowPrice["Count"];

                    TableInsert.Rows.Add(dataRowInsert);
                }

                con.Open();
                adapter.Update(TableInsert);
                con.Close();

                Console.WriteLine("Загрузил "+ TableInsert.Rows.Count+ " записей.");
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }

    /// <summary>
    /// Класс для описания поставщика и его колонок в прайсе
    /// </summary>
    class Suppliers
    {
        /// <summary>
        /// Уникальный номер поставщика
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название поставщика
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Email поставщика
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Имя поля "Производитель" в прайсе
        /// </summary>
        public string Vendor { get; set;}
        /// <summary>
        /// Имя поля "Каталожный номер" в прайсе
        /// </summary>
        public string Number { get; set;}
        /// <summary>
        /// Имя поля "Описание" в прайсе
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Имя поля "Цена" в прайсе
        /// </summary>
        public string Price { get; set; }
        /// <summary>
        /// Имя поля "Наличие" в прайсе
        /// </summary>
        public string Count { get; set; }
    }
}
