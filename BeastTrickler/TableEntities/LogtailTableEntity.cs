using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableEntities
{
    public class LogtailTableEntity : TableEntity
    {
        public Guid EmailId { get; set; }
        public string EmailAddress { get; set; }

        public LogtailTableEntity(string emailAddress)
        {
            PartitionKey = "LOGTAIL";
            RowKey = $"{DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks:D19}";
            EmailId = Guid.NewGuid();
            EmailAddress = emailAddress;
        }

        public LogtailTableEntity() { }

    }
}
