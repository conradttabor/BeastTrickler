using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableEntities
{
    public class SentTableEntity : TableEntity
    {
        public Guid EmailId { get; set; }
        public string EmailAddress { get; set; }
        public bool SuccessfullySent { get; set; }
    }
}
