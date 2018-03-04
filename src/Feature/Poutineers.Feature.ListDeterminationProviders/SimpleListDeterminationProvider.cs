/********************************************************************************
 * Poutineers 
 * 
 * The Poutineers love mentoring others and love crispy fries with fresh cheese curds covered with hot gravy to melt them just right.
 * 
 * LICENSE: The Poutineeers have developed this for Sitecore Hackathon to share with the world.
 *
 ********************************************************************************/

using Poutineers.Foundation.ListDeterminationProviders;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Poutineers.Feature.ListDeterminationProviders
{
    /// <summary>
    /// Interface implemented by all List Determination Providers
    /// </summary>
    public class SimpleListDeterminationProvider : IListDeterminationProvider
    {
        /// <summary>
        /// Additional Settings that could assist with determining the list
        /// </summary>
        public Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// Determines the list by processing the message passed in
        /// </summary>
        /// <param name="messageText"></param>
        /// <returns></returns>
        public string DetermineList(string messageText)
        {
            string listId = "4cb1336e-62b4-4ca6-c772-a88bb20b034d";
            if (messageText.ToUpperInvariant().Contains("COMMERCE"))
            {
                listId = ConfigurationManager.AppSettings["CommerceContactList"];
            }
            else if (messageText.ToUpperInvariant().Contains("POWERSHELL"))
            {
                listId = ConfigurationManager.AppSettings["PowershellContactList"];
            }
            else if (messageText.ToUpperInvariant().Contains("WFFM"))
            {
                listId = ConfigurationManager.AppSettings["WffmContactList"];
            }

            return listId;
        }
    }
}
