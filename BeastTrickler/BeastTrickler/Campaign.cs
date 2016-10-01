using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace BeastTrickler
{
    public class Campaign : TableEntity
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string FromDisplay { get; set; }          
        public string Template { get; set; }
        public DateTime BeginHours { get; set; }
        public DateTime EndHours { get; set; }
        public int MaxDailyEmails { get; set; }
        public string TrackinUrl { get; set; }
        public string ForwardingUrl { get; set; }
        public int SecondsBetweenSends { get; set; }
    }
}
