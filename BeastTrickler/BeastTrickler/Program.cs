using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeastTrickler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1 - Continue Sending Campaigns\n2 - Create a New Campaign\n3 - View a list of Campaigns\n4 - Load Emails\n5 - Quit");
            var input = Console.ReadLine();
            while (input != "5")
            {
                switch (input)
                {
                    case "1":
                        Console.WriteLine("What campaign do you want to begin?");
                        CampaignManager.BeginCampaign(Console.ReadLine());
                        break;
                    case "2":
                        CampaignCreator.CreateCampaign();
                        Console.ReadLine();
                        break;
                    case "3":
                        CampaignStorage.ReviewCampaigns();
                        Console.ReadLine();
                        break;
                    case "4":
                        BeastLoader.Program.Main();
                        Console.ReadLine();
                        break;
                    default:
                        Console.WriteLine($"{input} is not a valid entry");
                        Console.ReadLine();
                        break;
                }
                Console.WriteLine("1 - Continue Sending Campaigns\n2 - Create a New Campaign\n3 - View a list of Campaigns\n4 - Load Emails\n5 - Quit");
                input = Console.ReadLine();
            }
            
        }
    }
}
