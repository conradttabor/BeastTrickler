using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace BeastLoader
{
    public class EmailTableEntity : TableEntity
    {
        public Guid EmailId { get; set; }

        public EmailTableEntity(string emailAddress)
        {
            PartitionKey = "EMAILS";
            RowKey = emailAddress;
            EmailId = Guid.NewGuid();
        }

        public EmailTableEntity() { }
    }
}
