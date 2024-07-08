using CORNCafePOSAPI.Common;
using CORNCafePOSAPICommon;
namespace CORNCafePOSAPI.Model.Response
{
    public class BaseResponse
    {
        private E_ResponseReason _reason;

        public object Data { get; set; }
        public string Message { get; set; }
        public E_ResponseReason Reason
        {
            get
            {
                return _reason;
            }
            set
            {
                _reason = value;
                Message = _reason.ToDescription();
            }
        }

        public BaseResponse()
        {
            Data = new object();
            Message = string.Empty;
            Reason = E_ResponseReason.ERROR;
        }
        public BaseResponse(E_ResponseReason reason) : this()
        {
            Reason = reason;
        }
        public BaseResponse(string message) : this()
        {
            Message = message;
        }
        public BaseResponse(object data, E_ResponseReason reason) : this()
        {
            Data = data;
            Reason = reason;
        }
    }
}
