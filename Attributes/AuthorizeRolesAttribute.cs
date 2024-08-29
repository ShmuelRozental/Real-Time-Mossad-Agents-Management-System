using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Real_Time_Mossad_Agents_Management_System.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeRolesAttribute: Attribute
    {
        public string[] RolesById { get; }
        public AuthorizeRolesAttribute(params string[] rolesById)
        {
            RolesById = rolesById;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userIdClaim = context.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userRoles = context.HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToArray();

            if (!RolesById.Any(role => userRoles.Contains(role)))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
