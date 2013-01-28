using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;
using Orchard.Environment.Extensions;

namespace RM.QuickLogOn.OAuth.RU
{
    public abstract class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes()) routes.Add(routeDescriptor);
        }

        public abstract IEnumerable<RouteDescriptor> GetRoutes();
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.RU")]
    public class MailRuRoutes : Routes
    {
        public override IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor 
                {    
                    Priority = 10,
                    Route = new Route(
                        "QuickLogOn/MMAuth",
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth.RU"}, {"controller", "MailRuOAuth"}, {"action", "Auth"}, },
                        new RouteValueDictionary (),
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth.RU"} },
                        new MvcRouteHandler())
                }
            };
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.RU.Yandex")]
    public class YandexRoutes : Routes
    {
        public override IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Priority = 10,
                    Route = new Route(
                        "QuickLogOn/YXAuth",
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth.RU"}, {"controller", "YandexOAuth"}, {"action", "Auth"}, },
                        new RouteValueDictionary(),
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth.RU"} },
                        new MvcRouteHandler())

                }
            };
        }
    }
    
    [OrchardFeature("RM.QuickLogOn.OAuth.RU.VKontakte")]
    public class VKontakteRoutes : Routes
    {
        public override IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor 
                {    
                    Priority = 10,
                    Route = new Route(
                        "QuickLogOn/VKAuth",
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth.RU"}, {"controller", "VKontakteOAuth"}, {"action", "Auth"}, },
                        new RouteValueDictionary (),
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth.RU"} },
                        new MvcRouteHandler())
                }
            };
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.RU.Odnoklassniki")]
    public class OdnoklassnikiRoutes : Routes
    {
        public override IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor 
                {    
                    Priority = 10,
                    Route = new Route(
                        "QuickLogOn/OKAuth",
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth.RU"}, {"controller", "OdnoklassnikiOAuth"}, {"action", "Auth"}, },
                        new RouteValueDictionary (),
                        new RouteValueDictionary { {"area", "RM.QuickLogOn.OAuth.RU"} },
                        new MvcRouteHandler())
                }
            };
        }
    }
}