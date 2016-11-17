using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Web.Routing;

namespace Umbraco.Web.UI.Providers
{
    public class CustomUrlProvider : UrlProviderBase
    {
        //public CustomUrlProvider(IRequestHandlerSection requestSettings): base(requestSettings) { }

        public override IEnumerable<string> GetOtherUrls(UmbracoContext umbracoContext, int id, Uri current)
        {
            throw new NotImplementedException();
        }

        public override string GetUrl(UmbracoContext umbracoContext, int id, Uri current, UrlProviderMode mode)
        {
            var domainHelper = new DomainHelper(umbracoContext.Application.Services.DomainService);

            return AssembleUrl(domainHelper.DomainForNode(id, current), "/custom-url", current, mode).ToString();
        }
    }
}