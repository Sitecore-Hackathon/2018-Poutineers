using Poutineers.Feature.ArticleProviders;
using Poutineers.Foundation.ArticleProviders;
using PoutineersWebsite.Models;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.WebApi;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Schema;
using Sitecore.Xdb.Common.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PoutineersWebsite.Controllers
{
    public class SearchController : Controller
    {
        public ActionResult SlackSearch()
        {
            var model = new SlackSearchViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult SlackSearch(SlackSearchViewModel model) 
        {
            #region Setting things up for xConnect 

            // TO DO: This should be in app.config
            //var xConnectCertificateConnectionString = ConfigurationManager.AppSettings["xConnectCertificateConnectionString"];
            var xConnectCertificateConnectionString = "StoreName=My;StoreLocation=LocalMachine;FindType=FindByThumbprint;FindValue=3DE93B08A1C99FDC874B5A035EF02C66F3CB77E0";

            // (CTW) Getting xConnect Root Url from config
            //var xConnectRootUrl = ConfigurationManager.AppSettings["xConnectRootUrl"];
            var xConnectRootUrl = "https://hc.sc.xconnect";

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
                cfg.Initialize();
            }
            catch (XdbModelConflictException ce)
            {
                Console.WriteLine("ERROR:" + ce.Message);
                return null;
            }

            // Initialize a client using the validated configuration
            using (var client = new XConnectClient(cfg))
            {
                // TO DO: This should be loaded via reflection
                IArticleProvider articleProvider = new SimpleArticleProvider
                {
                    Client = client
                };

                model.ArticleList = articleProvider.GetArticles("SLACK_ALIAS", model.SlackId);

                return View("slacksearchview", model);
            }
        }
    }
}