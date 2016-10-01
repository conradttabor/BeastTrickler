using System;
using System.Configuration;
using System.IO;

namespace BeastLoader
{
    public class Program
    {
        public static void Main()
        {
            var fileLocation = ConfigurationManager.AppSettings["EmailFileLocation"];

            while (!File.Exists(fileLocation))
            {
                Console.WriteLine($"{fileLocation} does not exist. Enter a valid file path or type 'QUIT' to exit:");

                fileLocation = Console.ReadLine();

                if (fileLocation == "QUIT")
                    return;
            }

            if (fileLocation != null)
                using (var sr = File.OpenText(fileLocation))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        AzureTableStorer.StoreEmails(s);
                    }
                }

            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
