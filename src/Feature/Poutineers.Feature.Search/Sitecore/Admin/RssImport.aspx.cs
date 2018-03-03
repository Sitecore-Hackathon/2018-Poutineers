using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Sitecore.SecurityModel;
using Sitecore.Configuration;

namespace Poutineers.Feature.Search
{
    public partial class RssImport : System.Web.UI.Page
    {
        private static List<string> Blogs = new List<string>
        {
            "https://sitecorebasics.wordpress.com/feed/",
            "https://sitecorejunkie.com/feed/",
            "https://jammykam.wordpress.com/feed/",
            "https://citizensitecore.com/author/longhorntaco/feed/",
            "https://nshackblog.wordpress.com/feed/",
            "https://unaverhoeven.ghost.io/rss/",
            "https://www.kasaku.co.uk/atom.xml",
            "http://blog.ryanbailey.co.nz/feeds/posts/default?alt=rss",
            "https://blog.najmanowicz.com/feed/"
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (var blog in Blogs)
            {
                var reader = XmlReader.Create(blog);
                var articles = SyndicationFeed.Load(reader);
                reader.Close();

                foreach (SyndicationItem album in articles.Items)
                {
                    var title = album.Title.Text;
                    var summary = album.Summary.Text;
                    var articleLinkObject = album.Links.FirstOrDefault();
                    string articleLink = (articleLinkObject == null) ? null : articleLinkObject.Uri.AbsoluteUri;

                    Database masterDB = Factory.GetDatabase("master");

                    Item parentItem = masterDB.GetItem(global::Sitecore.Data.ID.Parse("{A0650ACD-7CFD-4FE9-B04E-E4A7FE9CDF02}"));

                    var template = masterDB.GetTemplate(global::Sitecore.Data.ID.Parse("{2073D960-9D65-4AC6-A94D-85E2883EC982}"));

                    using (new SecurityDisabler())
                    {
                        try
                        {
                            Item newItem = parentItem.Add(CleanItemName(title), template);
                            if (newItem != null)
                            {
                                newItem.Editing.BeginEdit();
                                newItem["Title"] = title;
                                newItem["Description"] = summary;
                                newItem["PostUrl"] = articleLink;

                                newItem.Editing.EndEdit();
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
        }

        public string CleanItemName(string name)
        {
            char[] invalidcharacters = Settings.InvalidItemNameChars;
            return string.Concat(name.Trim().Split(invalidcharacters));
        }
    }
}