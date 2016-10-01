using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace BeastLoader
{
    public class SentTableEntity : TableEntity
    {
        public Guid EmailId { get; set; }
        public string EmailAddress { get; set; }
    }
}
