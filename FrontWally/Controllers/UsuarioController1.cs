using FrontWally.DTOs.UsuarioDTOs;
using FrontWally.Helpers;
using FrontWally.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontWally.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApiService _apiService;
        public UsuarioController(ApiService apiService)
        {
            _apiService = apiService;
        }


        // Lista Usuario
        public async Task<IActionResult> Index()
        {
            var token = AuthHelper.ObtenerToken(User); // Obtener el token desde claims
            var usuarios = await _apiService.GetAllAsync<UsuarioDTO>("User/usuarios", token);
            return View(usuarios);
        }
    }
}
