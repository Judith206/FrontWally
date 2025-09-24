using System.Security.Claims;

namespace FrontWally.Helpers
{
    public static class AuthHelper
    {
        public static string ObtenerToken(ClaimsPrincipal user)
        {
            return user.FindFirstValue("Token") ?? string.Empty;
        }
    }

}
