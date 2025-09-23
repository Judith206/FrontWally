using FrontWally.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontWally.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // Get: mostrar login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}
