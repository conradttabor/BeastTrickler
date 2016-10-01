using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Threading;
using TableEntities;


namespace BeastTrickler
{
    public static class EmailSender
    {


        public static GmailService Authenticate()
        {
            string[] scopes = { GmailService.Scope.GmailCompose, GmailService.Scope.GmailSend };
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];

            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var credPath = ConfigurationManager.AppSettings["CredentialPath"];

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });

            return service;
        }

        public static bool SendEmail(string toAddress, string messageBody, string subject, GmailService service, Campaign campaign, LogtailTableEntity logtail)
        {
            messageBody = messageBody.Replace("{TrackingUrl}", campaign.TrackinUrl + "?" + "C_I=" + campaign.Id + "&E_I=" + logtail.EmailId);

            if (string.IsNullOrEmpty(campaign.FromDisplay)) campaign.FromDisplay = ConfigurationManager.AppSettings["SendingEmailAddress"];
            
            var msg = new AE.Net.Mail.MailMessage
            {
                Subject = subject,
                Body = messageBody,
                From = new MailAddress(ConfigurationManager.AppSettings["SendingEmailAddress"], campaign.FromDisplay),
                ContentType = "text/html",                                            
            };
            
            msg.To.Add(new MailAddress(toAddress));

            msg.ReplyTo.Add(msg.From); 

            var msgStr = new StringWriter();
            msg.Save(msgStr);

            var myEncodedstring = Base64UrlEncoder(msgStr.ToString());

            var success = false;
            const int maxRetryAttempts = 10;
            var attempts = 0;
            do
            {
                attempts++;
                Console.Write($"Sending to: {toAddress} [Attempt: {attempts}] ...");
                success = SendMessageWithRetry(service, myEncodedstring, attempts, maxRetryAttempts, toAddress);

                if (success) Console.WriteLine("Successful!");

                else if (attempts == maxRetryAttempts) break;

                else
                {
                    Console.WriteLine("     Waiting 30 seconds and will attempt again.");
                    System.Threading.Thread.Sleep(30000);
                }
                
            } while (!success);

            return success;
        }

        private static string Base64UrlEncoder(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);

            return Convert.ToBase64String(inputBytes)
              .Replace('+', '-')
              .Replace('/', '_')
              .Replace("=", "");
        }

        private static bool SendMessageWithRetry(GmailService service, string body, int attempts, int maxAttempts, string toAddress)
        {
            bool successful = false;

            try
            {
                service.Users.Messages.Send(new Message { Raw = body }, "me").Execute();
                successful = true;
            }
            catch (Exception ex)
            {
                Console.Beep();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed!");
                Console.WriteLine($"     Error message: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.Gray;
                if (attempts == maxAttempts) MaxAttemptsReached(ex, toAddress);
            }

            return successful;
        }

        private static void MaxAttemptsReached(Exception ex, string toAddress)
        {
            // Log exception and address in error file
        }
    }
}
