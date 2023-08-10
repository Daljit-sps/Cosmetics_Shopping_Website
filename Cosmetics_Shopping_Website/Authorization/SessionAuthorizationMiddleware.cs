using Microsoft.AspNetCore.Authorization;


namespace Cosmetics_Shopping_Website.Authorization
{

    public class SessionAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
       

        public SessionAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //Check if "AllowAnonymous" is present or not
            if (context.GetEndpoint()?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next.Invoke(context);
                return;
            }

            var session = context.Session;
            var key = session.GetString("UserData");

            if (string.IsNullOrEmpty(key))
            {

                //context.Request.Path = "/Users/Login";
                context.Response.Redirect("/Users/Login");
                //return;
                
            }

            // Continue to the next middleware
            await _next(context);
        }
    }

   

}
