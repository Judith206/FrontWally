using FrontWally.DTOs.UsuarioDTOs;
using System.Security.Claims;

namespace FrontWally.Helpers
{
    public class ClaimsHelper
    {
        public static ClaimsPrincipal CrearClaimsPrincipal(LoginResponseDTO usurio)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usurio.Nombre),
                new Claim(ClaimTypes.Email, usurio.Email),
                new Claim(ClaimTypes.Role, usurio.Rol),
                new Claim("Token", usurio.Token)
            };

            var identity = new ClaimsIdentity(claims, "AuthCookie");
            return new ClaimsPrincipal(identity);
        }
    }
}
