using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleLoadPriceEmail.Models
{
    /// <summary>
    /// Класс для описания поставщика
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
    }
}
