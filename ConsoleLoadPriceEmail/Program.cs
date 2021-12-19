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

            suppliers.Add(
                new Suppliers()
                {
                    Id = 1,
                    Name = "ООО доставим в срок",
                    Email = "strannik2292107@yandex.ru"
                });

            Email Email = new Email();
            string pathFile = Email.LoadPriceEmail(suppliers[0]);

            if (pathFile != null)
            {
                CSVFile CSVFile = new CSVFile();
                List<SuppliersPrice> suppliersPrices = CSVFile.LoadPriceCSVFile(pathFile);

                if (suppliersPrices != null)
                {
                    SqlQuery SqlQuery = new SqlQuery();
                    SqlQuery.RecordingPriceDb(suppliersPrices);
                }

            }
           
        }

      
    }

}
