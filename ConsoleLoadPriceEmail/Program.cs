using System;
using System.Collections.Generic;
using ConsoleLoadPriceEmail.Infrastructure;
using ConsoleLoadPriceEmail.SqlRequest;
using ConsoleLoadPriceEmail.Models;

namespace ConsoleLoadPriceEmail
{
    class Program
    {

        static void Main(string[] args)
        {
            List<Suppliers> suppliers = new List<Suppliers>();

            //грузим поставщиков
            suppliers.Add(
                new Suppliers()
                {
                    Id = 1,
                    Name = "ООО доставим в срок",
                    Email = "test@yandex.ru"
                });

            
            Email Email = new Email();
            //Выбираем поставщика
            //и грузим последнее письмо
            string pathFile = Email.LoadPriceEmail(suppliers[0]);

            if (pathFile != null)
            {
                ReadCSVFile readCSVFile = new ReadCSVFile();

                //получаем список позиций
                List<SuppliersPrice> suppliersPrices = readCSVFile.LoadPriceCSVFile(pathFile);


                if (suppliersPrices != null)
                {
                    SqlQuery SqlQuery = new SqlQuery();
                    //пишем позиции в базу
                    SqlQuery.RecordingPriceDb(suppliersPrices);
                }

            }
           
        }

      
    }

}
