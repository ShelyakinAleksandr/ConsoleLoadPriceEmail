using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConsoleLoadPriceEmail.Models;

namespace ConsoleLoadPriceEmail.Infrastructure
{
    class Email
    {
        /// <summary>
        /// Метод для загрузки последнего прайса присланного поставщиком.
        /// Метод принимает на вход структуру Suppliers.
        /// </summary>
        /// <param name="suppliers"></param>
        /// <returns>Возвращает путь к скачанному файлу</returns>
        public string LoadPriceEmail(Suppliers suppliers)
        {
            string pathToPrice = null;

            try
            {
                Console.WriteLine("Подключаюсь к почте");

                using (ImapClient client = new ImapClient())
                {
                    client.Connect("imap.yandex.ru", 993, true);
                    
                    client.Authenticate("test@yandex.ru", "pass");

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

                    return pathToPrice;
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            return null;
        }
    }
}
