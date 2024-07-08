using System.Net;
using System.Text;
using CORNCafePOSAPI.Authorization;
using CORNCafePOSAPICommon;
using CORNCafePOSAPI.Common;
using CORNCafePOSAPI.Model.Request;
using CORNCafePOSAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CORNCafePOSAPI.Controllers
{
    [ApiController]    
    public class DefaultController : BaseController
    {
        [AllowAnonymous]
        [HttpGet("")]
        public string Test()
        {
            return "Welcome to ASP.NET Core Minimal CORNCafePOSAPI";
        }

        [AllowAnonymous]
        [HttpGet("MultiTenant/GetClientInfo")]
        public async Task<ObjectResult> GetClientInfo(string pin)
        {
            try
            {
                var repository = new CommonRepository(UserId, "");
                var response = await repository.GetClientInfo(pin);

                return SetResponse(response);
            }
            catch (Exception ex)
            {
                return SetErrorResponse(ex);
            }
        }

        [AllowAnonymous]
        [HttpPost("Token")]
        public async Task<ObjectResult> Token(CommonRequest request, [FromServices] IJwtUtils jwtUtils)
        {
            try
            {
                var repository = new CommonRepository(UserId, ConnectionString2);
                var response = await repository.Login(request);

                if (response.Reason != E_ResponseReason.SUCCESS)
                {
                    return SetErrorResponse(response.Message);
                }

                var userId = Convert.ToInt64(response.Data.GetExpandoPropertyValue("UserId"));
                var userInfo = response.Data.GetExpandoPropertyValue("UserInfo");
                var token = jwtUtils.GenerateJwtToken(userId);

                token.UserInfo = userInfo;

                return SetSuccessResponse(token);
            }
            catch (Exception ex)
            {
                return SetErrorResponse(ex);
            }
        }

        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public ObjectResult RefreshToken([FromServices] IJwtUtils jwtUtils)
        {
            try
            {
                if (UserId <= 0)
                {
                    return SetUnauthorizeResponse(E_ResponseReason.INVALID_TOKEN.ToDescription());
                }

                var token = jwtUtils.GenerateJwtToken(UserId);

                return SetSuccessResponse(token);
            }
            catch (Exception ex)
            {
                return SetErrorResponse(ex);
            }
        }

        [Authorize]
        [HttpPost("Common/ExecSp")]
        public async Task<ObjectResult> ExecSp(CommonRequest request)
        {
            try
            {
                var repository = new CommonRepository(UserId, ConnectionString2);
                var response = await repository.ExecSP(request);

                return SetResponse(response);
            }
            catch (Exception ex)
            {
                return SetErrorResponse(ex);
            }
        }
       
        [AllowAnonymous]
        [HttpPost("Common/SendCode")]
        public async Task<ObjectResult> SendCode(OTPCodeRequest request)
        {
            try
            {
                 Random generator = new Random();
                int r = generator.Next(100000, 1000000);

                var repository = new CommonRepository(UserId, ConnectionString2);
                if(SendSMS(r.ToString(),request.CustomerPhoneNo))
                {
                    var response = await repository.UpdateCustomerOTPCode(request,r.ToString());
                    return SetResponse(response);
                }
                else{
                    return SetSuccessResponseMessage("SMS not sent");
                }
            }
            catch (Exception ex)
            {
                return SetErrorResponse(ex);
            }
        }

        [AllowAnonymous]
        [HttpPost("Common/VerifyCustomerOTPCode")]
        public async Task<ObjectResult> VerifyCustomerOTPCode(OTPCodeRequest request)
        {
            try
            {
                var repository = new CommonRepository(UserId, ConnectionString2);
                
                    var response = await repository.GetCustomerByOTPCode(request);
                    return SetResponse(response);
            }
            catch (Exception ex)
            {
                return SetErrorResponse(ex);
            }
        }

        private static void WriteLog(string Msg)
        {
            using (FileStream fs = System.IO.File.Open("C:\\CORNPOSKOTPrintLog.txt", FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(string.Format("{0}: {1}", DateTime.Now, Msg));
                    sw.Write(Environment.NewLine);
                }
            }
        }
        private bool SendSMS(string OTPCode,string PhoneNo)
        {
            bool flag = true;
            String result = "";
            String strPost = "id=rchfastservice&pass=Ra*wa6as91&msg=" + OTPCode + "&to=" + PhoneNo + "" + "&mask=CornPOS&type=xml&lang=English";
            StreamWriter myWriter = null;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create("https://outreach.pk/api/sendsms.php/sendsms/url");
            objRequest.Method = "POST";
            objRequest.ContentLength = Encoding.UTF8.GetByteCount(strPost);
            objRequest.ContentType = "application/x-www-form-urlencoded";
            try
            {
                
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(strPost);
            }
            catch (Exception e)
            {
                
            }
            finally
            {
                myWriter.Close();
            }
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                sr.Close();
            }
            var xml2 = System.Xml.Linq.XElement.Parse(result);
            if (xml2.Elements("type").FirstOrDefault().Value.ToLower() == "success")
            {
                flag = true;
            }
            else
            {
                flag=false;
            }
            return flag;
        }
    }
}