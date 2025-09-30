using FrontWally.DTOs.UsuarioDTOs;
using System.Security.Claims;

namespace FrontWally.Helpers
{
    public class ClaimsHelper
    {
        public static ClaimsPrincipal CrearClaimsPrincipal(LoginResponseDTO usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("Token", usuario.Token),
                new Claim("UserId", usuario.Id.ToString()) // ✅ Este es el claim que faltaba
            };

            var identity = new ClaimsIdentity(claims, "AuthCookie");
            return new ClaimsPrincipal(identity);
        }
    }
}
