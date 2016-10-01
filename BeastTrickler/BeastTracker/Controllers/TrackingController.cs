using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BeastTracker.Models;
using Microsoft.ApplicationInsights;

namespace BeastTracker.Controllers
{
    public class TrackingController : Controller
    {
        public ActionResult Tracked(string C_I, string E_I)
        {
            try
            {
                Guid campaignId;
                if (C_I == null || E_I == null || !Guid.TryParse(C_I, out campaignId)) return Redirect(System.Configuration.ConfigurationManager.AppSettings["ForwardingAddress"]);

                var click = new ClickHistoryEntity()
                {
                    PartitionKey = E_I,
                    RowKey = $"{DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks:D19}",
                    CampaignId = campaignId,
                    ClickTime = DateTime.UtcNow,
                    IpAddress = HttpContext.Request.ServerVariables["REMOTE_ADDR"],
                };
                TrackingStorage.InsertEmail("CLICKHISTORY", click);
            }
            catch (Exception ex)
            {
                var telemetry = new TelemetryClient();


                    var properties = new Dictionary<string, string>
                        {{"CampaignId", C_I}, {"EmailId", E_I}};

                    telemetry.TrackException(ex, properties);

            }
            return Redirect(System.Configuration.ConfigurationManager.AppSettings["ForwardingAddress"]);
        }
    }
}