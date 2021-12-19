using ConsoleLoadPriceEmail.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;


namespace ConsoleLoadPriceEmail.Infrastructure
{
    class CSVFile
    {
        public List<SuppliersPrice> LoadPriceCSVFile( string pathToPrice)
        {
            List<SuppliersPrice> suppliersPrice = new List<SuppliersPrice>();

            try
            {
                int nomberRows = 1;

                if (pathToPrice != null)
                {
                    List<string> listEror = new List<string>();

                    Console.WriteLine("Скачал, открываю и читаю");

                    var isRecordBad = false;
                    var config = new CsvConfiguration(CultureInfo.CurrentCulture)
                    {
                        Delimiter = ";",
                        BadDataFound = context => { isRecordBad = true; }
                    };

                    using (StreamReader reader = new StreamReader(new FileStream(pathToPrice, FileMode.Open)))
                    {
                        using (var csv = new CsvReader(reader, config))
                        {
                            csv.Read();
                            csv.ReadHeader();

                            while (csv.Read())
                            {
                                nomberRows++;
                                try { suppliersPrice.Add(csv.GetRecord<SuppliersPrice>()); }
                                catch (Exception) { listEror.Add("строка " + nomberRows); }

                                if (isRecordBad)
                                    listEror.Add("строка " + nomberRows);
                                isRecordBad = false;
                            }
                        }
                    }

                    Console.WriteLine("Прочитал" + nomberRows);
                    Console.WriteLine("Не смог прочитать " + listEror.Count + " позиций");
                }

                return suppliersPrice;

            }
            catch (FileNotFoundException) { Console.WriteLine("Ошибка, не смог найти скачаный файл."); }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            return null;
        }


    }
}
