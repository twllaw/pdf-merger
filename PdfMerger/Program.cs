using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace ConsoleApp1
{
    class Program
    {
        static string _path;

        static void Main(string[] args)
        {
            _path = ConfigurationManager.AppSettings["PdfFolderPath"];

            Console.WriteLine($"Enter the directory with the PDF files to merge (press enter to use default location {_path}):");

            var directoryInput = Console.ReadLine();

            if (string.IsNullOrEmpty(directoryInput))
            {
                Console.WriteLine("Continuing with the default directory.");
            }
            else if (!string.IsNullOrEmpty(directoryInput) && !Directory.Exists(directoryInput))
            {
                Console.WriteLine("Directory does not exist! Continuing with the default directory.");
            }
            else
            {
                _path = directoryInput;
            }

            var fileList = GetFiles();

            if (fileList == null || !fileList.Any())
            {
                Console.WriteLine($"No files to process.");
                Console.ReadKey();
                return;
            }

            var output = new PdfDocument();

            foreach (var file in fileList)
            {
                var doc = PdfReader.Open(file, PdfDocumentOpenMode.Import);

                var count = doc.PageCount;
                for (int pageNum = 0; pageNum < count; pageNum++)
                {
                    output.AddPage(doc.Pages[pageNum]);
                }
            }

            Console.WriteLine($"Enter your desired file name for the merged file:");

            var fileNameInput = Console.ReadLine();
            output.Save($"{_path}\\{fileNameInput}.pdf");

            Console.WriteLine("PDFs successfully merged!");
            Console.ReadKey();        
        }

        private static IEnumerable<string> GetFiles()
        {
            if (!Directory.Exists(_path))
            {
                Console.WriteLine($"Directory {_path} does not exist! Please double check that the config is correct.");
                return null;
            }

            var files = Directory.GetFiles($"{_path}")?
                .Where(file => file.Contains(".pdf"))
                .OrderBy(file => file);

            return files;
        }
    }
}
