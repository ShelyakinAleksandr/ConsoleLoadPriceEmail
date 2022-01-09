using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ConsoleLoadPriceEmail.Models;

namespace ConsoleLoadPriceEmail.Infrastructure
{
    class ReadCSVFile
    {
        public List<SuppliersPrice> LoadPriceCSVFile(string pathToPrice)
        {
            List<SuppliersPrice> listSuppliers = new List<SuppliersPrice>();

            if (pathToPrice != null)
            {
                List<string> listEror = new List<string>();

                Console.WriteLine("Скачал, открываю и читаю");

                using (StreamReader reader = new StreamReader(new FileStream(pathToPrice, FileMode.Open)))
                {

                    #region формируем имена столбцов
                    //Считываем первую строку
                    //Она же Заголовки столбцов
                    string column = reader.ReadLine();
                    string[] columnMass = column.Split(';');

                    //С помощью данного листа связываю структуру CSV файла со структурой SuppliersPrice
                    List<SuppliersColumn> columnNumber = new List<SuppliersColumn> 
                        { new SuppliersColumn { IdSuppliers = 1, NomberColumn = 0, NameColumn = "Бренд", NameParametr = "Vendor" },
                          new SuppliersColumn { IdSuppliers = 1, NomberColumn = 0, NameColumn = "Каталожный номер", NameParametr = "Number" },
                          new SuppliersColumn { IdSuppliers = 1, NomberColumn = 0, NameColumn = "Описание", NameParametr = "Description" },
                          new SuppliersColumn { IdSuppliers = 1, NomberColumn = 0, NameColumn = "Цена, руб.", NameParametr = "Price" },
                          new SuppliersColumn { IdSuppliers = 1, NomberColumn = 0, NameColumn = "Наличие", NameParametr = "Count" },
                    };

                    //ищу в CSV файле столбцы соответствующие NameColumn и запоминаю их номера
                    foreach (SuppliersColumn suppliersColumn in columnNumber)
                    {
                        for (int i=0; i<columnMass.Length; i++)
                        {
                            if (suppliersColumn.NameColumn == columnMass[i])
                                suppliersColumn.NomberColumn = i;
                        }
                    }

                    #endregion

                    //читаю CSV файл и создаю на каждую позицию структуру SuppliersPrice
                    string row;
                    while ((row = reader.ReadLine()) != null)
                    {

                        string[] valuesRow = row.Split(';');

                        //гружу структуру в List
                        listSuppliers.Add(
                            new SuppliersPrice { Vendor = valuesRow[(columnNumber.FirstOrDefault(col => col.NameParametr == "Vendor")).NomberColumn],
                                                 Count = valuesRow[(columnNumber.FirstOrDefault(col => col.NameParametr == "Count")).NomberColumn],
                                                 Description = valuesRow[(columnNumber.FirstOrDefault(col => col.NameParametr == "Description")).NomberColumn],
                                                 Number = valuesRow[(columnNumber.FirstOrDefault(col => col.NameParametr == "Number")).NomberColumn],
                                                 Price = valuesRow[(columnNumber.FirstOrDefault(col => col.NameParametr == "Price")).NomberColumn]
                            });
                    }

                }
                Console.WriteLine(listSuppliers.Count);
                return listSuppliers;
            }

            return null;
        }
    }
}
