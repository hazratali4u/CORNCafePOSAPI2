namespace CORNCafePOSAPI.Model
{
    public class ClientInfo : BaseModel
    {
        public long ClientId { get; set; }
        public string? ClientConnString { get; set; }
        public string? ImagePath1 { get; set; }
        public string? ImagePath2 { get; set; }
        public string? ImageServerUrl { get; set; }
    }
}
