using CORNCafePOSAPICommon;
using CORNCafePOSAPI.Model.Response;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace CORNCafePOSAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        protected int UserId
        {
            get
            {
                return Convert.ToInt32(HttpContext.Items["UserId"]);
            }
        }
        protected string ConnectionString2
        {
            get
            {
                return MyUtilityMethod.Decrypt(Request.Headers["x-conn"]);
            }
        }

        protected ObjectResult SetResponse(BaseResponse response)
        {
            return response.Reason == E_ResponseReason.SUCCESS ? SetSuccessResponse(response.Data) : SetErrorResponse(response.Message);
        }
        protected OkObjectResult SetSuccessResponse(object source)
        {
            if (source.GetType().Namespace == typeof(List<object>).Namespace ||
                source.GetType() == typeof(bool) ||
                source.GetType() == typeof(string) ||
                source.GetType() == typeof(int))
            {
                dynamic srcData = new ExpandoObject();

                srcData.Data = source;
                source = srcData;
            }

            dynamic data = source;

            data.Status = true;

            return new OkObjectResult(data);
        }
        protected OkObjectResult SetSuccessResponseMessage(string message)
        {
            dynamic data = new ExpandoObject();

            data.Message = message;
            data.Status = true;

            return new OkObjectResult(data);
        }
        protected BadRequestObjectResult SetErrorResponse(string message)
        {
            dynamic data = new ExpandoObject();

            data.Message = message;
            data.Status = false;

            return new BadRequestObjectResult(data);
        }
        protected BadRequestObjectResult SetErrorResponse(Exception ex)
        {
            var message = ex.Message;

            //if (ex is SqlException)
            //{
            //    message = E_ResponseReason.DATABASE_ERROR.ToDescription();
            //}

            return SetErrorResponse(message);
        }
        protected UnauthorizedObjectResult SetUnauthorizeResponse(string message)
        {
            dynamic data = new ExpandoObject();

            data.Message = message;
            data.Status = false;

            return new UnauthorizedObjectResult(data);
        }
    }
}
