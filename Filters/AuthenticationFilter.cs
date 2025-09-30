using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CQCDMS.Filters
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Skip authentication check for Login controller
            if (context.Controller.GetType().Name == "LoginController")
            {
                base.OnActionExecuting(context);
                return;
            }

            // Check if user is logged in
            var isLoggedIn = context.HttpContext.Session.GetString("IsLoggedIn");
            
            if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
            {
                // User is not logged in, redirect to login page
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
