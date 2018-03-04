/********************************************************************************
 * Poutineers 
 * 
 * The Poutineers love mentoring others and love crispy fries with fresh cheese curds covered with hot gravy to melt them just right.
 * 
 * LICENSE: The Poutineeers have developed this for Sitecore Hackathon to share with the world.
 *
 ********************************************************************************/

using Newtonsoft.Json.Linq;
using Poutineers.Feature.ListDeterminationProviders;
using Poutineers.Feature.Slack;
using Poutineers.Feature.XConnect;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.WebApi;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Schema;
using Sitecore.Xdb.Common.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PoutineersSlackToXConnectConsole
{
    class Program
    {
        private static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("");
            Console.WriteLine("END OF PROGRAM.");
            Console.ReadKey();
        }

        private static async Task MainAsync(string[] args)
        {
            bool UnitTesting = false;

            #region Setting things up for xConnect 

            // (CTW) Getting xConnect settings from config
            var xConnectCertificateConnectionString = ConfigurationManager.AppSettings["xConnectCertificateConnectionString"];
            var xConnectRootUrl = ConfigurationManager.AppSettings["xConnectRootUrl"];

            CertificateWebRequestHandlerModifierOptions options =
            CertificateWebRequestHandlerModifierOptions.Parse(xConnectCertificateConnectionString);
            var certificateModifier = new CertificateWebRequestHandlerModifier(options);

            List<IHttpClientModifier> clientModifiers = new List<IHttpClientModifier>();
            var timeoutClientModifier = new TimeoutHttpClientModifier(new TimeSpan(0, 0, 20));
            clientModifiers.Add(timeoutClientModifier);

            var collectionClient = new CollectionWebApiClient(new Uri(xConnectRootUrl + "/odata"), clientModifiers, new[] { certificateModifier });
            var searchClient = new SearchWebApiClient(new Uri(xConnectRootUrl + "/odata"), clientModifiers, new[] { certificateModifier });
            var configurationClient = new ConfigurationWebApiClient(new Uri(xConnectRootUrl + "/configuration"), clientModifiers, new[] { certificateModifier });

            var cfg = new XConnectClientConfiguration(
            new XdbRuntimeModel(CollectionModel.Model), collectionClient, searchClient, configurationClient);

            #endregion

            try
            {
                await cfg.InitializeAsync();

                // Print Poutine if configuration is valid (Thanks Martina)
                var arr = new[]
                {
                    @"__________              __  .__                                   ",
                    @"\______   \____  __ ___/  |_|__| ____   ____   ___________  ______",
                    @" |     ___/  _ \|  |  \   __\  |/    \_/ __ \_/ __ \_  __ \/  ___/",
                    @" |    |  (  <_> )  |  /|  | |  |   |  \  ___/\  ___/|  | \/\___ \ ",
                    @" |____|   \____/|____/ |__| |__|___|  /\___  >\___  >__|  /____  >",
                    @"                                    \/     \/     \/           \/ "
                };

                Console.WindowWidth = 160;
                foreach (string line in arr)
                    Console.WriteLine(line);

            }
            catch (XdbModelConflictException ce)
            {
                Console.WriteLine("ERROR:" + ce.Message);
                return;
            }

            // Initialize a client using the validated configuration
            using (var client = new XConnectClient(cfg))
            {
                while (1 == 1)
                {
                    try
                    {
                        #region Slack Message Onboarding

                        var slackManager = new SlackManager
                        {
                            SlackRootUrl = ConfigurationManager.AppSettings["SlackRootUrl"],
                            OAuthToken = ConfigurationManager.AppSettings["SlackOAuthToken"]
                        };

                        var messagesJson = string.Empty;
                        if (UnitTesting)
                        {
                            // If unit testing you can use this to debug
                            messagesJson = "{\"ok\":true,\"messages\":[{\"type\":\"message\",\"user\":\"U9HVD2DMJ\",\"text\":\"Ok Poutineers this message is about sitecore commerce\",\"ts\":\"1520047207.000024\"},{\"user\":\"U9HVD2DMJ\",\"text\":\"<@U9HVD2DMJ> has joined the channel\",\"type\":\"message\",\"subtype\":\"channel_join\",\"ts\":\"1520045546.000096\"},{\"user\":\"U9HT438RZ\",\"text\":\"<@U9HT438RZ> has joined the channel\",\"type\":\"message\",\"subtype\":\"channel_join\",\"ts\":\"1520044764.000002\"},{\"user\":\"U9HVAPFMJ\",\"text\":\"<@U9HVAPFMJ> has joined the channel\",\"type\":\"message\",\"subtype\":\"channel_join\",\"ts\":\"1520044582.000044\"}],\"has_more\":false}";
                        }
                        else
                        {
                            messagesJson = slackManager.GetChannelHistory(ConfigurationManager.AppSettings["SlackChannelName"]);
                        }


                        XConnectManager xConnectManager = new XConnectManager
                        {
                            Client = client
                        };

                        // Use JObject to parse it
                        var o = JObject.Parse(messagesJson);
                        foreach (var message in o["messages"].Children())
                        {
                            var userId = message["user"].ToString();
                            var text = message["text"].ToString();
                            var ts = message["ts"].ToString();
                            var messageText = message.ToString();

                            // TO DO: This is where the provider gets loaded
                            #region List Determination Code

                            // passes in additional meta that may assist more advanced providers determine the list
                            var settings = new Dictionary<string, string>
                            {
                                { "full_history", o["messages"].ToString() }
                            };

                            // TO DO: Look up the right provider to load

                            // If not provider specified use the simple one.
                            var simpleListDeterminationProvider = new SimpleListDeterminationProvider();
                            string listId = simpleListDeterminationProvider.DetermineList(messageText);

                            #endregion

                            // Look up the contact in xDB
                            Contact contact = xConnectManager.GetContactByIndentifier("SLACK", userId, true);

                            // if the contact does not exist then we need to load the profile
                            if (contact == null)
                            {
                                var userJson = slackManager.GetUserProfile(userId);
                                var oUser = JObject.Parse(userJson);

                                // WARNING: Assuming the way name is parsed. Should really be looking at algorithm for name split.
                                var nameParts = oUser["profile"]["display_name"].ToString().Split(' ');

                                // Save the new contact
                                xConnectManager.InsertContactAndAddContactToList("SLACK", userId, oUser["profile"]["real_name"].ToString(), nameParts[0], (nameParts.Length > 1 ? nameParts[1] : ""), oUser["profile"]["title"].ToString(), oUser["profile"]["email"].ToString(), listId);
                            }
                            else
                            {
                                if (contact.Identifiers.FirstOrDefault(x => x.Source.ToString().ToUpperInvariant() == "SLACK_ALIAS") == null)
                                {
                                    var userJson = slackManager.GetUserProfile(userId);
                                    var oUser = JObject.Parse(userJson);
                                    client.AddContactIdentifier(contact, new ContactIdentifier("SLACK_ALIAS", oUser["profile"]["real_name"].ToString(), ContactIdentifierType.Known));
                                }

                                // Since contact exists it is possible they already have message so need to check for duplicates.
                                if (contact.ListSubscriptions().Subscriptions.FirstOrDefault(x => x.ListDefinitionId == new Guid(listId)) == null)
                                {
                                    // Add the interaction to the contact
                                    xConnectManager.AddContactToList(contact, listId);
                                }
                            }
                        }

                        #endregion

                        // We will check slack and then rest
                        Console.WriteLine("Waiting");
                        Thread.Sleep(300000);
                    }
                    catch (XdbExecutionException ex)
                    {
                        // Deal with exception
                    }
                }
            }
        }
    }
}