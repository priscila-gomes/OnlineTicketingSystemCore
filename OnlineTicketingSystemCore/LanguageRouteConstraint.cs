using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Globalization;
using System.Threading;

namespace OnlineTicketingSystemCore
{
    public class LanguageRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route,string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (!values.ContainsKey("lang"))
            {
                return false;
            }

            var lang = values["lang"].ToString();

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

            return lang == "en" || lang == "fr";
        }
    }
}
