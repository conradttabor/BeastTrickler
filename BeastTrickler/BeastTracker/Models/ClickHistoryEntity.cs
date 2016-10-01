using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace BeastTracker.Models
{
    public class ClickHistoryEntity : TableEntity
    {
        public Guid CampaignId { get; set; }
        public string IpAddress { get; set; }
        public DateTime ClickTime { get; set; }
        public ClickHistoryEntity() {}
    }
}