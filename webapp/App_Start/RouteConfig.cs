#region Using

using System.Web.Mvc;
using System.Web.Routing;

#endregion

namespace eSPP
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.LowercaseUrls = true;
            routes.MapRoute("EditModule", "UserGroupBak/EditByModule/{id}/{moduleId}", new
            {
                controller = "UserGroupBak",
                action = "EditByModule",
                moduleId = UrlParameter.Optional,
                id = UrlParameter.Optional
            }).RouteHandler = new DashRouteHandler();
            routes.MapRoute("Default", "{controller}/{action}/{id}", new
            {
                controller = "Home",
                action = "Index",
                id = UrlParameter.Optional
            }).RouteHandler = new DashRouteHandler();
        }
    }
}