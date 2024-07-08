namespace CORNCafePOSAPI.Model.Request
{
    public class CommonRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }

        public string? SpName { get; set; }
        public Dictionary<string, object>? Parameters { get; set; }
    }
    public class OTPCodeRequest
    {
        public string? CustomerPhoneNo {get;set;}
        public string? OTPCode {get;set;}
    }
}
