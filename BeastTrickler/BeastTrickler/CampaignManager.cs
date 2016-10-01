using System;
using System.Linq;
using BeastLoader;
using Google.Apis.Gmail.v1;
using Microsoft.WindowsAzure.Storage.Table;
using TableEntities;

namespace BeastTrickler
{
    public static class CampaignManager
    {
        public static void BeginCampaign(string campaignName)
        {
            var campaign = CampaignStorage.GetCampaignByName(campaignName);
            var gmailService = EmailSender.Authenticate();

            if (campaign == null)
            {
                Console.WriteLine("This campaign does not exist in the table!");
                return;
            }

            bool continueCampaign = true;
            int counter = 0;
            while (continueCampaign)
            {
                continueCampaign = RunCampaign(campaign, gmailService);

                // Conrad: I added this to see how many have been sent so far.
                // After every 10 sends, it will add blue line of text
                // with the current count.
                counter++;
                if (counter%10 == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Processed {counter} so far ...");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            // TODO : Limit to max per day
            // Conrad: Now that I've introduced the counter variable (above)
            // this should be trivial.  You'll just need to reset that
            // variable for each "session" ... i.e., between 8am and 4pm,
            // for example.
            Console.WriteLine("No more new emails in the table! Please load more and rerun the application.");
            Console.ReadLine();
        }

        private static bool RunCampaign(Campaign campaign, GmailService gmailService)
        {
            while (DateTime.Now.TimeOfDay < campaign.BeginHours.TimeOfDay ||
                   DateTime.Now.TimeOfDay > campaign.EndHours.TimeOfDay)
            {
                Console.WriteLine(
                    $"Waiting, {DateTime.Now.TimeOfDay:g} is not between {campaign.BeginHours.TimeOfDay:g} and {campaign.EndHours.TimeOfDay:g}");
                System.Threading.Thread.Sleep(5000);
            }

            var moreEmails = DripEmail(campaign, gmailService);

            System.Threading.Thread.Sleep(campaign.SecondsBetweenSends*1000);

            if (!moreEmails) return false;

            return true;
        }

        private static bool DripEmail(Campaign campaign, GmailService gmailService)
        {
            var query1 = new TableQuery<SentTableEntity>().Take(1);

            var lastSentEntry = BeastLoader.AzureTableStorer.GetSentEntries(query1, "SENT").FirstOrDefault();

            TableQuery<LogtailTableEntity> query2;

            if (lastSentEntry == null) query2 = new TableQuery<LogtailTableEntity>();

            else
            {
                query2 =
                    new TableQuery<LogtailTableEntity>()
                        .Where(TableQuery.GenerateFilterCondition(
                            "RowKey",
                            QueryComparisons.LessThan,
                            lastSentEntry.RowKey));
            }


            var logtail = BeastLoader.AzureTableStorer.GetLogtailEntries(query2, "LOGTAIL").LastOrDefault();

            if (logtail != null)
            {
                var nextEmail = logtail;
                var sent = new SentTableEntity()
                {
                    EmailAddress = nextEmail.EmailAddress,
                    EmailId = nextEmail.EmailId,
                    PartitionKey = "SENT",
                    RowKey = nextEmail.RowKey
                };

                var success = EmailSender.SendEmail(nextEmail.EmailAddress, campaign.Template, campaign.Subject, gmailService, campaign, nextEmail);

                sent.SuccessfullySent = success;
                AzureTableStorer.InsertEmail("SENT", sent);
            }

            else
            {
                Console.WriteLine("No more new emails");
                return false;
            }

            return true;
        }
    }
}
