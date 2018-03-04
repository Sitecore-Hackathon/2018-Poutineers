/********************************************************************************
 * Poutineers 
 * 
 * The Poutineers love mentoring others and love crispy fries with fresh cheese curds covered with hot gravy to melt them just right.
 * 
 * LICENSE: The Poutineeers have developed this for Sitecore Hackathon to share with the world.
 *
 ********************************************************************************/

using Poutineers.Feature.Search.Helpers;
using Poutineers.Feature.XConnect;
using Poutineers.Foundation.ArticleProviders;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Poutineers.Feature.ArticleProviders
{
    /// <summary>
    /// This is the simplest article provider.
    /// </summary>
    public class SimpleArticleProvider : IArticleProvider
    {
        /// <summary>
        /// The xConnectClient to interact with
        /// </summary>
        public XConnectClient Client { get; set; }

        /// <summary>
        /// Gets the articles based on the id and other criteria.
        /// </summary>
        /// <param name="idType">Type of identifier eg SLACK_ALIAS</param>
        /// <param name="id">The actual identifier</param>
        /// <returns></returns>
        /// <remarks>The simple provider only returns 1 link per category.</remarks>
        public List<SearchHelper.SearchResult> GetArticles(string idType, string id)
        {
            var xConnectManager = new XConnectManager
            {
                Client = Client
            };

            Contact contact = xConnectManager.GetContactByIndentifier(idType, id, true);
            if (contact.ListSubscriptions() != null)
            {
                if (contact.ListSubscriptions().Subscriptions.Count > 0)
                {
                    // TO DO: Get articles based on the list. Hard coded in the simple version to take list name and do search.
                    foreach(var list in contact.ListSubscriptions().Subscriptions)
                    {
                        // TO DO: Dynamically load the list from list manager
                        SearchHelper.SearchResults searchResults = new SearchHelper.SearchResults();
                        switch (list.ListDefinitionId.ToString().ToUpperInvariant())
                        {
                            // CommerceContactList
                            case "38E9C0B0-ED57-49C6-9714-79F1B09159C2":
                                searchResults = SearchHelper.DoSearch("commerce, sitecore commerce, commerce server");
                                break;
                            // PowershellContactList
                            case "D2C719E6-D9AC-42EF-E2B6-263137430FE0":
                                searchResults = SearchHelper.DoSearch("powershell, spe, sitecore powershell");
                                break;
                            // WffmList
                            case "1314234D-4785-4D8C-C9A3-0AF2045B3184":
                                break;
                        }

                        return searchResults.Results;
                    }
                }
            }

            return new List<SearchHelper.SearchResult>();
        }
    }
}
