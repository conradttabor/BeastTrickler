using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableEntities
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
