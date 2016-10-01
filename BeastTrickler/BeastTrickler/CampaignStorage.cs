using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace BeastTrickler
{
    public static class CampaignStorage
    {

        public static void ReviewCampaigns()
        {
            var campaigns = GetAllCampaigns();

            foreach (var campaign in campaigns)
            {
                Console.WriteLine("------------------");
                Console.WriteLine($"Name: {campaign.Name}");
                Console.WriteLine($"Template Location: {campaign.Template}");
                Console.WriteLine($"Time: {campaign.BeginHours.Hour:00}:{campaign.BeginHours.Minute:00} - {campaign.EndHours.Hour:00}:{campaign.EndHours.Minute:00}");
                Console.WriteLine($"Forwarding URL: {campaign.ForwardingUrl}");
                Console.WriteLine($"Tracking URL: {campaign.TrackinUrl}");
                Console.WriteLine($"Seconds Between Sends: {campaign.SecondsBetweenSends}");
                Console.WriteLine($"Max Daily Emails: {campaign.MaxDailyEmails}");
                Console.WriteLine("------------------");
            }
        }

        private static IEnumerable<Campaign> GetAllCampaigns()
        {
            var table = GetCloudTable();
            TableQuery<Campaign> query = new TableQuery<Campaign>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "CAMPAIGNS"));

            var campaigns = table.ExecuteQuery(query);

            return campaigns;
        }

        public static Campaign GetCampaignByName(string name)
        {
            var table = GetCloudTable();
            var doesEntryExist = DoesEntryExist(name);
            TableQuery<Campaign> query = new TableQuery<Campaign>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, name));
            var campaign = table.ExecuteQuery(query).FirstOrDefault();
            return campaign;
        }

        public static void StoreCampaign(Campaign campaign)
        {
            if (DoesEntryExist(campaign))
            {
                Console.WriteLine($"{campaign.Name} is already in the Table! Please choose a different name.");
                return;
            }

            InsertEntity(campaign);

            Console.WriteLine($"{campaign.Name} was created successfully!");
        }

        private static bool DoesEntryExist(string name)
        {
            var table = GetCloudTable();
            var query =
                new TableQuery<Campaign>()
                    .Where(TableQuery.GenerateFilterCondition(
                        "RowKey",
                        QueryComparisons.Equal,
                        name));

            if (table.ExecuteQuery(query).Any()) return true;

            return false;
        }

        private static bool DoesEntryExist(ITableEntity entity)
        {
            var table = GetCloudTable();
            var query =
                new TableQuery<Campaign>()
                    .Where(TableQuery.GenerateFilterCondition(
                        "RowKey",
                        QueryComparisons.Equal,
                        entity.RowKey));

            if (table.ExecuteQuery(query).Any())
            {
                Console.WriteLine($"{entity.RowKey} is already in the table!");
                return true;
            }

            return false;
        }

        private static CloudTable GetCloudTable()
        {
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("CAMPAIGNS");
            table.CreateIfNotExists();

            return table;
        }

        private static void InsertEntity(ITableEntity entity)
        {
            try
            {
                var table = GetCloudTable();

                var insertOperation = TableOperation.Insert(entity);
                table.Execute(insertOperation);

            }
            catch (Exception e)
            {
                using (var file =
                    new StreamWriter(@"ErrorLog.txt", true))
                {
                    Console.WriteLine($"Error: {e.InnerException}");
                    file.WriteLine(e.Message);
                }
            }
        }
    }
}
