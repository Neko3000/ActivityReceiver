using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ActivityReceiver.Functions
{
    //changed HtmlHelper => IHtmlHelper
    public static class HtmlHelpers
    {
        // Bool
        public static bool IsInControllerAction(this IHtmlHelper htmlHelper, string controller, string action)
        {
            var routeData = htmlHelper.ViewContext.RouteData;

            var routeAction = routeData.Values["action"].ToString();
            var routeController = routeData.Values["controller"].ToString();

            return controller == routeController && action == routeAction;
        }

        public static bool IsInController(this IHtmlHelper htmlHelper, string controller)
        {
            var routeData = htmlHelper.ViewContext.RouteData;

            var routeAction = routeData.Values["action"].ToString();
            var routeController = routeData.Values["controller"].ToString();

            return controller == routeController;
        }

        public static bool IsInControllers(this IHtmlHelper htmlHelper, params string[] controllers)
        {
            var routeData = htmlHelper.ViewContext.RouteData;

            var routeAction = routeData.Values["action"].ToString();
            var routeController = routeData.Values["controller"].ToString();

            var isExistedInControllers = false;
            foreach (var controller in controllers)
            {
                if (controller == routeController)
                {
                    isExistedInControllers = true;
                    break;
                }
            }

            return isExistedInControllers;
        }

        // String
        public static string IsActive(this IHtmlHelper htmlHelper, string controller, string action)
        {
            var routeData = htmlHelper.ViewContext.RouteData;

            var routeAction = routeData.Values["action"].ToString();
            var routeController = routeData.Values["controller"].ToString();

            return (controller == routeController && action == routeAction) ? "active" : "";
        }

        public static string IsActiveForController(this IHtmlHelper htmlHelper,string controller)
        {
            var routeData = htmlHelper.ViewContext.RouteData;

            var routeAction = routeData.Values["action"].ToString();
            var routeController = routeData.Values["controller"].ToString();

            return controller == routeController ? "active" : "";
        }
        public static string IsActiveForController(this IHtmlHelper htmlHelper,params string[] controllers)
        {
            var routeData = htmlHelper.ViewContext.RouteData;

            var routeAction = routeData.Values["action"].ToString();
            var routeController = routeData.Values["controller"].ToString();

            var isExistedInControllers = false;
            foreach(var controller in controllers)
            {
                if(controller==routeController)
                {
                    isExistedInControllers = true;
                    break;
                }
            }

            return isExistedInControllers ? "active" : "";
        }
    }
}