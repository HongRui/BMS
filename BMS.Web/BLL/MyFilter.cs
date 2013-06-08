using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMS.Web.Models;
using OUDAL;
namespace BMS.Web
{
    public class MyFilter : FilterAttribute, IActionFilter, IResultFilter, IExceptionFilter, IAuthorizationFilter
    {
        #region IActionFilter 成员

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //filterContext.RequestContext.HttpContext.Response.Write(string.Format("Action({0})已经执行了!<br />"
            //    ,filterContext.ActionDescriptor.ActionName));
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //filterContext.RequestContext.HttpContext.Response.Write(string.Format("Action({0})执行之前!<br />"
            //    ,filterContext.ActionDescriptor.ActionName));
        }

        #endregion


        #region IResultFilter 成员

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            //filterContext.RequestContext.HttpContext.Response.Write("Result已经执行了!");
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //filterContext.RequestContext.HttpContext.Response.Write("Result执行之前!");
        }

        #endregion

        #region IExceptionFilter 成员

        public void OnException(ExceptionContext filterContext)
        {
            //string controller = filterContext.RouteData.Values["controller"] as string;
            //string action = filterContext.RouteData.Values["action"] as string;

            //filterContext.RequestContext.HttpContext.Response.Write(string.Format("{0}:{1}发生异常!{2}",
            //    controller,action, filterContext.Exception.Message));
            //filterContext.ExceptionHandled = true;
        }

        #endregion

        #region IAuthorizationFilter 成员

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //if (filterContext.HttpContext.User.Identity.IsAuthenticated == false)
            //{
            //    if(filterContext.HttpContext.Application["Login"] as string =="password")
            //    { 
            //        if (filterContext.ActionDescriptor.ActionName != "LogOn")
            //        {
            //            filterContext.HttpContext.Response.Redirect("~/Account/LogOn?ReturnUrl=" + filterContext.HttpContext.Request.Url.PathAndQuery);
            //        }
            //    }else
            //    {
            //        filterContext.HttpContext.Response.Redirect("~/Content/AccessDeny.htm");
            //    }
                
            //    //
            //}
        }

        #endregion
    }
}