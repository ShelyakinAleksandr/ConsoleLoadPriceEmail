using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleLoadPriceEmail.Models
{
    class SuppliersPrice
    {
        private string description = null;
        private int count = 0;
        private string vendor = null;
        private string number = null;
        private string searchVendor = null;
        private string searchNumber = null;


        /// <summary>
        /// Имя поля "Бренд" в прайсе
        /// </summary>
        [Name("Бренд")]
        public string Vendor
        {
            get => vendor;
            set
            {
                vendor = value;
                SearchVendor = value;
            }
        }
        /// <summary>
        /// Имя поля "номер запчасти" в прайсе
        /// </summary>

        [Name("Каталожный номер")]
        public string Number
        {
            get => number;
            set
            {
                number = value;
                SearchNumber = value;
            }
        }

        /// <summary>
        /// Имя поля "Описание" в прайсе
        /// </summary>
        [Name("Описание")]
        public string Description
        {
            get { return description; }

            set
            {
                if (512 < value.Length)
                    description = value.Substring(0, 512);
                else
                    description = value;
            }
        }

        /// <summary>
        /// Имя поля "Цена" в прайсе
        /// </summary>
        [Name("Цена, руб.")]
        public string Price { get; set; }

        /// <summary>
        /// Имя поля "Наличие" в прайсе
        /// </summary>
        [Name("Наличие")]
        public int Count
        {
            get { return count; }
            set
            {
                try { count = value; }
                catch (CsvHelper.TypeConversion.TypeConverterException)
                {
                    try
                    {
                        string count = Convert.ToString(value);

                        if (count.Contains('-'))
                            value = Convert.ToInt32(count.Substring(count.IndexOf('-') + 1));

                        if (count.Contains('>'))
                            value = Convert.ToInt32(count.Trim('>'));

                        if (count.Contains('<'))
                            value = Convert.ToInt32(count.Trim('<'));
                    }
                    finally { count = 0; }
                }
            }
        }

        /// <summary>
        /// Данное поле преобразует значение поля Vendor убирая из него все не ЦифроБуквенные символы
        /// </summary>
        [Ignore]
        public string SearchVendor
        {
            get => searchVendor;
            set => searchVendor = new string(value.Where(c => Char.IsLetter(c) || Char.IsDigit(c)).ToArray()).ToUpper();
        }

        /// <summary>
        /// Данное поле преобразует значение поля Number убирая из него все не ЦифроБуквенные символы
        /// </summary>
        [Ignore]
        public string SearchNumber
        {
            get => searchNumber;
            set => searchNumber = new string(value.Where(c => Char.IsLetter(c) || Char.IsDigit(c)).ToArray()).ToUpper();
        }
    }
}
