using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace TchotchoLoto.Controllers
{
    public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {


            if (controllerContext.HttpContext.Request.IsAjaxRequest())
            {
                return true;
            }


            controllerContext.HttpContext.Response.Redirect("/Account/Home");
            return false;
        }
    }
}