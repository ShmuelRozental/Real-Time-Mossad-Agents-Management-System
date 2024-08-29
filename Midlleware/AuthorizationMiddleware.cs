using Real_Time_Mossad_Agents_Management_System.Attributes;
using System.IdentityModel.Tokens.Jwt;


namespace Real_Time_Mossad_Agents_Management_System.Midlleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            var endPoint = context.GetEndpoint();
            if (endPoint != null)
            {
                var requiredRoles = endPoint.Metadata
                    .OfType<AuthorizeRolesAttribute>()
                    .SelectMany(atrr => atrr.RolesById).ToArray();

                if (requiredRoles.Length > 0)
                {
                   
                    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                    if (authHeader != null && authHeader.StartsWith("Bearer "))
                    {
                        var token = authHeader.Substring("Bearer ".Length).Trim();

                        var tokenHandler = new JwtSecurityTokenHandler();
                        try
                        {
                            var jwtToken = tokenHandler.ReadJwtToken(token);

                            // Extract the user ID from the token claims
                            var userId = jwtToken.Claims
                                .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

                            if (string.IsNullOrEmpty(userId) || !requiredRoles.Contains(userId))
                            {
                                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                                await context.Response.WriteAsync("Access denied. User does not have the required role.");
                                return;
                            }
                        }
                        catch (Exception)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync("Invalid token.");
                            return;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Missing token.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}