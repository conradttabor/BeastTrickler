using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace BeastTracker
{
    public static class TrackingStorage
    {
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

    }
}