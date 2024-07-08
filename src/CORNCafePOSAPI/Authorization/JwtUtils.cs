using CORNCafePOSAPICommon;
using CORNCafePOSAPI.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CORNCafePOSAPI.Authorization
{
    public interface IJwtUtils
    {
        public AuthToken GenerateJwtToken(long userId);
        public int? ValidateJwtToken(string token);
    }

    public class JwtUtils : IJwtUtils
    {
        public JwtUtils()
        { }

        public AuthToken GenerateJwtToken(long userId)
        {
            var issued = DateTime.Now;
            var expires = issued.AddMinutes(Cache.Jwt_ExpiryDurationInMin);
            var claims = new[]
            {
                new Claim("id", userId.ToString())
             };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Cache.Jwt_Key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(Cache.Jwt_Issuer, Cache.Jwt_Issuer, claims, expires: expires, signingCredentials: credentials);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            var token = new AuthToken
            {
                Access_Token = accessToken,
                Token_Type = "bearer",
                Expires_In = Cache.Jwt_ExpiryDurationInMin,
                Issued = issued,
                Expires = expires
            };

            return token;
        }

        public int? ValidateJwtToken(string token)
        {
            try
            {
                new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Cache.Jwt_Issuer,
                    ValidAudience = Cache.Jwt_Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Cache.Jwt_Key!))
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}
