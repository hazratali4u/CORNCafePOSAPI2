namespace CORNCafePOSAPICommon
{
    public static class Cache
    {
        public static string? DBConnectionString { get; set; }
        public static string? Jwt_Key { get; set; }
        public static string? Jwt_Issuer { get; set; }
        public static int Jwt_ExpiryDurationInMin { get; set; }
    }
}
