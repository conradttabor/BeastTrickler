using System;
using System.IO;

namespace BeastLoader
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Enter the desired email file:");
            var fileLocation = Console.ReadLine();

            while (!File.Exists(fileLocation))
            {
                Console.WriteLine("File does not exist. Enter a valid file path or type 'QUIT' to exit:");

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
