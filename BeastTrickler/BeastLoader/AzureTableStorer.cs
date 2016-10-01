using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using TableEntities;

namespace BeastLoader
{
    public static class AzureTableStorer
    {

        public static void StoreEmails(string email)
        {
            var emailEntity = new EmailTableEntity(email);
            var emailIdEntity = new EmailIdTableEntity(email);
            var logtailEntity = new LogtailTableEntity(email);

            try
            {
                if (DoesEntryExist(emailEntity))
                    return;

                InsertEmail("EMAIL", emailEntity);
                InsertEmail("EMAILID", emailIdEntity);
                InsertEmail("LOGTAIL", logtailEntity);

                Console.WriteLine($"{email} was entered successfully!");
            }

            catch (Exception e)
            {
                using (var file =
                    new StreamWriter(@"ErrorLog.txt", true))
                {
                    file.WriteLine(e.Message);
                }
            }
        }

        public static void InsertEmail(string cloudTable, ITableEntity entity)
        {
            var table = GetCloudTable(cloudTable);
            var insertOperation = TableOperation.Insert(entity);
            table.Execute(insertOperation);
        }

        private static CloudTable GetCloudTable(string desiredTable)
        {
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(desiredTable);
            table.CreateIfNotExists();
            
            return table;
        }

        public static IEnumerable<LogtailTableEntity> GetLogtailEntries(TableQuery<LogtailTableEntity> query, string table)
        {
            var cloudTable = GetCloudTable(table);
            var entries = cloudTable.ExecuteQuery(query);
            return entries;
        }

        public static IEnumerable<SentTableEntity> GetSentEntries(TableQuery<SentTableEntity> query, string table)
        {
            var cloudTable = GetCloudTable(table);
            var entries = cloudTable.ExecuteQuery(query);
            return entries;
        }

        private static bool DoesEntryExist(ITableEntity entity)
        {
            var table = GetCloudTable("EMAIL");
            var query =
                new TableQuery<EmailTableEntity>()
                    .Where(TableQuery.GenerateFilterCondition(
                        "RowKey",
                        QueryComparisons.Equal,
                        entity.RowKey));

            if ((entity.GetType().Name == "EmailTableEntity") && table.ExecuteQuery(query).Any())
            {
                Console.WriteLine($"{entity.RowKey} is already in the table!");
                return true;
            }

            return false;
        }
    }
}
