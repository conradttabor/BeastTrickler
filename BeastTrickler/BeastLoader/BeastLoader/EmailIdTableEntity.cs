using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace BeastLoader
{
    public class EmailIdTableEntity : TableEntity
    {
        public string EmailAddress { get; set; }

        public EmailIdTableEntity(string emailAddress)
        {
            PartitionKey= "GUIDS";
            RowKey = Guid.NewGuid().ToString();
            EmailAddress = emailAddress;
        }

        public EmailIdTableEntity() { }
    }
}
