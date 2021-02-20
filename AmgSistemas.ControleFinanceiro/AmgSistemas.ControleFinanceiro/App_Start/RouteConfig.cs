using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AmgSistemas.ControleFinanceiro
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Logar", id = UrlParameter.Optional }
            );

           
            routes.MapRoute(
               name: "Registro2",
               url: "{controller}/{action}/{id}/{identificador}/{identificador2}",
               defaults: new { controller = "Login", action = "Logar", id = UrlParameter.Optional, identificador = UrlParameter.Optional, identificador2 = UrlParameter.Optional }
           );
                     

            routes.MapRoute(
              name: "Busca",
              url: "{controller}/{action}/{id}/{ano}/{mes}",
              defaults: new { controller = "Login", action = "Logar", id = UrlParameter.Optional, ano = UrlParameter.Optional, mes = UrlParameter.Optional }
          );

        }
    }
}
