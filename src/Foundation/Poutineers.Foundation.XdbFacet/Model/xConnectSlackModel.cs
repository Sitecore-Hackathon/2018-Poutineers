using Sitecore.XConnect.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect;

namespace Poutineers.Foundation.XdbFacet.Model
{
    public class xConnectSlackModel
    {
        public static XdbModel Model { get; } = BuildModel();

        private static XdbModel BuildModel()
        {
            var builder = new XdbModelBuilder("xConnectSlackModel", new XdbModelVersion(1, 0));
            builder.ReferenceModel(CollectionModel.Model);
            builder.DefineFacet<Contact, SlackInformation>(SlackInformation.FacetName);

            return builder.BuildModel();
        }
    }
}