using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PoutineersWebsite.Models
{
    /// <summary>
    /// Module for Slack Article Search
    /// </summary>
    public class SlackSearchViewModel
    {
        /// <summary>
        /// Id representing the Slack account
        /// </summary>
        public string SlackId { get; set; }

        /// <summary>
        /// List of articles
        /// </summary>
        public List<string> ArticleList { get; set; }
    }
}