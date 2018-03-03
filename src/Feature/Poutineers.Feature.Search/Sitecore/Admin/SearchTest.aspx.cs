using Poutineers.Feature.Search.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Poutineers.Feature.Search.Sitecore.Admin
{
    public partial class SearchTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var results = SearchHelper.DoSearch("commerce");
        }
    }
}