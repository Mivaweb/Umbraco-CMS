using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;

namespace Umbraco.Web.Routing
{
    /// <summary>
    /// Implements a base class for url providers
    /// </summary>
    public abstract class UrlProviderBase : IUrlProvider
    {
        private readonly IRequestHandlerSection _requestSettings;

        [Obsolete("Use the ctor that specifies the IRequestHandlerSection")]
        public UrlProviderBase()
            : this(UmbracoConfig.For.UmbracoSettings().RequestHandler)
        {
        }

        public UrlProviderBase(IRequestHandlerSection requestSettings)
        {
            _requestSettings = requestSettings;
        }

        #region Abstract methods

        public abstract IEnumerable<string> GetOtherUrls(UmbracoContext umbracoContext, int id, Uri current);
        public abstract string GetUrl(UmbracoContext umbracoContext, int id, Uri current, UrlProviderMode mode);

        #endregion

        #region Utilities

        protected Uri AssembleUrl(DomainAndUri domainUri, string path, Uri current, UrlProviderMode mode)
        {
            Uri uri;

            // ignore vdir at that point, UriFromUmbraco will do it

            if (mode == UrlProviderMode.AutoLegacy)
            {
                mode = _requestSettings.UseDomainPrefixes
                    ? UrlProviderMode.Absolute
                    : UrlProviderMode.Auto;
            }

            if (domainUri == null) // no domain was found
            {
                if (current == null)
                    mode = UrlProviderMode.Relative; // best we can do

                switch (mode)
                {
                    case UrlProviderMode.Absolute:
                        uri = new Uri(current.GetLeftPart(UriPartial.Authority) + path);
                        break;
                    case UrlProviderMode.Relative:
                    case UrlProviderMode.Auto:
                        uri = new Uri(path, UriKind.Relative);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("mode");
                }
            }
            else // a domain was found
            {
                if (mode == UrlProviderMode.Auto)
                {
                    if (current != null && domainUri.Uri.GetLeftPart(UriPartial.Authority) == current.GetLeftPart(UriPartial.Authority))
                        mode = UrlProviderMode.Relative;
                    else
                        mode = UrlProviderMode.Absolute;
                }

                switch (mode)
                {
                    case UrlProviderMode.Absolute:
                        uri = new Uri(CombinePaths(domainUri.Uri.GetLeftPart(UriPartial.Path), path));
                        break;
                    case UrlProviderMode.Relative:
                        uri = new Uri(CombinePaths(domainUri.Uri.AbsolutePath, path), UriKind.Relative);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("mode");
                }
            }

            // UriFromUmbraco will handle vdir
            // meaning it will add vdir into domain urls too!
            return UriUtility.UriFromUmbraco(uri);
        }
        
        // always build absolute urls unless we really cannot
        protected IEnumerable<Uri> AssembleUrls(IEnumerable<DomainAndUri> domainUris, string path)
        {
            // no domain == no "other" url
            if (domainUris == null)
                return Enumerable.Empty<Uri>();

            // if no domain was found and then we have no "other" url
            // else return absolute urls, ignoring vdir at that point
            var uris = domainUris.Select(domainUri => new Uri(CombinePaths(domainUri.Uri.GetLeftPart(UriPartial.Path), path)));

            // UriFromUmbraco will handle vdir
            // meaning it will add vdir into domain urls too!
            return uris.Select(UriUtility.UriFromUmbraco);
        }

        // Combines paths
        string CombinePaths(string path1, string path2)
        {
            string path = path1.TrimEnd('/') + path2;
            return path == "/" ? path : path.TrimEnd('/');
        }

        #endregion
    }
}