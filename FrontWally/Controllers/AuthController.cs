using FrontWally.DTOs.UsuarioDTOs;
using FrontWally.Helpers;
using FrontWally.Services;
using Microsoft.AspNetCore.Authentication;
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

        //POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(UsuarioLoginDTO dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
            {
                ViewBag.Error = "Credenciales Invalidas";
                return View();
            }
            // Crear y firmar los claims usando el helper

            var principal = ClaimsHelper.CrearClaimsPrincipal(result);

            await HttpContext.SignInAsync("AuthCookie", principal);

            return RedirectToAction("Index", "Home");
        }

        // POST: registro
        [HttpPost]
        public async Task<IActionResult> Registrar(UsuarioRegistroDTO dto)
        {
            var result = await _authService.RegistrarAsync(dto);

            if (result == null || result.Id <= 0)
            {
                ViewBag.Error = "Error al registrar el usuario";
                return View("Registro");

            }
            // Crear y firmar los claims usando el helper
            var principal = ClaimsHelper.CrearClaimsPrincipal(result);

            await HttpContext.SignInAsync("AuthCookie", principal);

            return RedirectToAction("Index", "Home");
        }

        // Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AuthCookie");
            return RedirectToAction("Login");
        }
        //GET: Mostrar Registro
        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }
    }
}
