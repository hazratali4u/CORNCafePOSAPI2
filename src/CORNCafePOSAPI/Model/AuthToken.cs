namespace CORNCafePOSAPI.Model
{
    public class AuthToken : BaseModel
    {
        public string? Access_Token { get; set; }
        public string? Token_Type { get; set; }
        public long Expires_In { get; set; }
        public DateTime Issued { get; set; }
        public DateTime Expires { get; set; }
        public dynamic? UserInfo { get; set; }
    }
}
