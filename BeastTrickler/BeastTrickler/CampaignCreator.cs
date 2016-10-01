using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeastTrickler
{
    static class CampaignCreator
    {
        public static void CreateCampaign()
        {
            var campaign = new Campaign {PartitionKey = "CAMPAIGNS"};

            Console.WriteLine("New Campaign Name:");
            campaign.Name = Console.ReadLine();

            Console.WriteLine("Email Subject:");
            campaign.Subject = Console.ReadLine();

            Console.WriteLine("Display name on the From Address:");
            campaign.FromDisplay = Console.ReadLine();

            campaign.Id = Guid.NewGuid();

            Console.WriteLine("File location of Template Email:");
            var fileLocation = Console.ReadLine();

            while (!File.Exists(fileLocation))
            {
                Console.WriteLine("No file found there! Please enter a valid file location:");
                fileLocation = Console.ReadLine();
            }

            if (fileLocation != null)
            {
                using (var sr = File.OpenText(fileLocation))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        campaign.Template += s;
                    }
                }
            }
            

            Console.WriteLine("Campaign begin time: (HH:mm)");
            DateTime beginTime;
            var validTime = DateTime.TryParse(Console.ReadLine(), out beginTime);

            while (!validTime)
            {
                Console.WriteLine("Not a valid time! Please enter a valid time in the format HH:mm :");
                validTime = DateTime.TryParse(Console.ReadLine(), out beginTime);
            }

            campaign.BeginHours = beginTime;

            Console.WriteLine("Campaign end time: (HH:mm)");
            DateTime endTime;
            validTime = DateTime.TryParse(Console.ReadLine(), out endTime);

            while (!validTime)
            {
                Console.WriteLine("Not a valid time! Please enter a valid time in the format HH:mm :");
                validTime = DateTime.TryParse(Console.ReadLine(), out beginTime);
            }

            campaign.EndHours = endTime;

            Console.WriteLine("Maximum number of daily emails:");
            int maxEmails;
            var validInput = int.TryParse(Console.ReadLine(), out maxEmails);

            while (!validInput)
            {
                Console.WriteLine("Not a valid input! Please enter an intiger value for the maximum numbers of emails to send:");
                validInput = int.TryParse(Console.ReadLine(), out maxEmails);
            }

            campaign.MaxDailyEmails = maxEmails;

            Console.WriteLine("Tracking URL:");
            campaign.TrackinUrl = Console.ReadLine();

            Console.WriteLine("forwarding URL:");
            campaign.ForwardingUrl = Console.ReadLine();

            Console.WriteLine("Seconds between sends:");
            int sendTime;
            validInput = int.TryParse(Console.ReadLine(), out sendTime);

            while (!validInput)
            {
                Console.WriteLine("Not a valid input! Please enter an intiger value for the time between email sends:");
                validInput = int.TryParse(Console.ReadLine(), out sendTime);
            }

            campaign.SecondsBetweenSends = sendTime;

            campaign.RowKey = campaign.Name;

            CampaignStorage.StoreCampaign(campaign);

            Console.WriteLine("Done!");
            
        }
    }
}
