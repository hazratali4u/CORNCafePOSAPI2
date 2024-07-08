using System.ComponentModel;

namespace CORNCafePOSAPICommon
{
    public enum E_ResponseReason
    {
        [Description("None")]
        NONE = -1,

        [Description("Error")]
        ERROR = 0,

        [Description("Success")]
        SUCCESS = 1,

        [Description("Empty Parameters")]
        EMPTY_PARAMETERS = 2,

        [Description("Invalid Parameters")]
        INVALID_PARAMETERS = 3,

        [Description("Database Error")]
        DATABASE_ERROR = 4,

        [Description("Data not Found")]
        DATA_NOT_FOUND = 5,

        [Description("Invalid Pin")]
        INVALID_PIN = 6,

        [Description("Invalid Username or Password")]
        INVALID_USERNAME_PASSWORD = 7,

        [Description("Invalid Token")]
        INVALID_TOKEN = 8,
    }
}
