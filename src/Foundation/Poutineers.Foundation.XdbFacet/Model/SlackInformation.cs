using Sitecore.XConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Poutineers.Foundation.XdbFacet.Model
{
    [FacetKey(FacetName)]
    public class SlackInformation : Facet
    {
        public const string FacetName = "SlackInformation";

        public string SlackId { get; set; }
        public string SlackMessages { get; set; }
    }
}