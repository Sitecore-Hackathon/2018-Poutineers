/********************************************************************************
 * Poutineers 
 * 
 * The Poutineers love mentoring others and love crispy fries with fresh cheese curds covered with hot gravy to melt them just right.
 * 
 * LICENSE: The Poutineeers have developed this for Sitecore Hackathon to share with the world.
 *
 ********************************************************************************/

using System.Collections.Generic;

namespace Poutineers.Foundation.ListDeterminationProviders
{
    /// <summary>
    /// Interface implemented by all List Determination Providers
    /// </summary>
    public interface IListDeterminationProvider
    {
        /// <summary>
        /// Additional Settings that could assist with determining the list
        /// </summary>
        Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// Determines the list by processing the message passed in
        /// </summary>
        /// <param name="messageText"></param>
        /// <returns></returns>
        string DetermineList(string messageText);
    }
}
