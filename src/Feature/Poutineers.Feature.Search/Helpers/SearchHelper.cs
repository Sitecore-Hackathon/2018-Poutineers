using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Poutineers.Feature.Search.Helpers
{
    public class SearchHelper
    {
        /// <summary>
        /// Complete a search
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>Search results object</returns>
        public static SearchResults DoSearch(string searchTerm)
        {
            var myResults = new SearchResults
            {
                Results = new List<SearchResult>()
            };

            var searchIndex = ContentSearchManager.GetIndex("poutineers_posts_index"); // Get the search index
            var searchPredicate = GetSearchPredicate(searchTerm); // Build the search predicate

            using (var searchContext = searchIndex.CreateSearchContext()) // Get a context of the search index
            {
                var searchResults = searchContext.GetQueryable<SearchModel>().Where(searchPredicate); // Search the index for items which match the predicate

                // This will get all of the results, which is not reccomended
                var fullResults = searchResults.GetResults();

                foreach (var hit in fullResults.Hits)
                {
                    myResults.Results.Add(new SearchResult
                    {
                        Title = hit.Document.ItemName,
                        Url = hit.Document.ArticleUrl
                    });
                }

                return myResults;
            }
        }

        /// <summary>
        /// Search logic
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>Search predicate object</returns>
        public static Expression<Func<SearchModel, bool>> GetSearchPredicate(string searchTerm)
        {
            var predicate = PredicateBuilder.True<SearchModel>(); // Items which meet the predicate

            foreach (var t in searchTerm.Split(','))
            {
                predicate = predicate.Or(x => x.Title.Contains(t.Trim())).Boost(1.0f);
                predicate = predicate.Or(x => x.Description.Contains(t.Trim())).Boost(1.0f);
            }

            return predicate;
        }

        /// <summary>
        /// Search item mapped to SOLR index
        /// </summary>
        public class SearchModel : SearchResultItem
        {
            [IndexField("_name")]
            public virtual string ItemName { get; set; }

            [IndexField("_displayname")]
            public virtual string DispalyName { get; set; }

            [IndexField("description")]
            public virtual string Description { get; set; }

            [IndexField("title")]
            public virtual string Title { get; set; }

            [IndexField("posturl")]
            public virtual string ArticleUrl { get; set; } 
        }

        /// <summary>
        /// Custom search result model for binding to front end
        /// </summary>
        public class SearchResult
        {
            public string Title { get; set; }

            public string Url { get; set; }
        }

        /// <summary>
        /// Custom search result model for binding to front end
        /// </summary>
        public class SearchResults
        {
            public List<SearchResult> Results { get; set; }
        }
    }
}