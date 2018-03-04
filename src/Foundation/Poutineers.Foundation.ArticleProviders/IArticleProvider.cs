/********************************************************************************
 * Poutineers 
 * 
 * The Poutineers love mentoring others and love crispy fries with fresh cheese curds covered with hot gravy to melt them just right.
 * 
 * LICENSE: The Poutineeers have developed this for Sitecore Hackathon to share with the world.
 *
 ********************************************************************************/

using Poutineers.Feature.Search.Helpers;
using System.Collections.Generic;

namespace Poutineers.Foundation.ArticleProviders
{
    /// <summary>
    /// Interface implementeed by all Article Providers
    /// </summary>
    public interface IArticleProvider
    {
        /// <summary>
        /// Gets articles links personalized for the given user
        /// </summary>
        /// <param name="idType">Type of ID (for now only support slack_alias</param>
        /// <param name="id">The Id</param>
        /// <returns></returns>
        List<SearchHelper.SearchResult> GetArticles(string idType, string id);
    }
}
