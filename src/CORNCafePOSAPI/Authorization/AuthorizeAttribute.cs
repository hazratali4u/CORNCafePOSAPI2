using CORNCafePOSAPI.Common;
using CORNCafePOSAPICommon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CORNCafePOSAPI.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();

            if (allowAnonymous)
            {
                return;
            }

            var userId = Convert.ToInt64(context.HttpContext.Items["UserId"]);

            if (userId <= 0)
            {
                context.Result = new JsonResult(new
                {
                    Message = E_ResponseReason.INVALID_TOKEN.ToDescription(),
                    Status = false
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
        }
    }
}
