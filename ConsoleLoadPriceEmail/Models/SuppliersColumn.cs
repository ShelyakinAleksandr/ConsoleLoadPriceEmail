using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleLoadPriceEmail.Models
{
    /// <summary>
    /// Связующее звено между CSV файлом и структурой SuppliersPrice, которое позволяет, после загрузки, считать CSV файл 
    /// </summary>
    class SuppliersColumn
    {
        /// <summary>
        /// Уникальный номер поставщика
        /// </summary>
        public int IdSuppliers { get; set; }
        /// <summary>
        /// номер столбца в CSV файле
        /// </summary>
        public int NomberColumn { get; set; }
        /// <summary>
        /// Название столбца в CSV файле
        /// </summary>
        public string NameColumn { get; set; }
        /// <summary>
        /// название параметра в структуре SuppliersPrice
        /// </summary>
        public string NameParametr { get; set; }
    }
}
