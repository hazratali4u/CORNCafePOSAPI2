namespace CORNCafePOSAPI.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IJwtUtils jwtUtils)
        {
            var arr = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ");
            var tokenType = arr?.Length < 2 ? null : arr?[0];
            var token = arr?.Length < 2 ? null : arr?[1];

            if (tokenType != "bearer" || token == null)
            {
                await _next(context);

                return;
            }

            var userId = jwtUtils.ValidateJwtToken(token);

            if (userId != null)
            {
                context.Items["UserId"] = userId.Value;
            }

            await _next(context);
        }
    }
}
