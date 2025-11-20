using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FrontWally.Models;
using Microsoft.AspNetCore.Authorization;
using FrontWally.Services;
using FrontWally.DTOs.ProductoDTOs;

namespace FrontWally.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ProductoService _productoService;
    private readonly CotizacionService _cotizacionService;

    public HomeController(
        ILogger<HomeController> logger,
        ProductoService productoService,
        CotizacionService cotizacionService)
    {
        _logger = logger;
        _productoService = productoService;
        _cotizacionService = cotizacionService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var token = await GetTokenAsync();

            // Obtener productos recientes
            var productos = await _productoService.GetAllProductosAsync(token);
            var productosRecientes = productos
                .OrderByDescending(p => p.Id) // O por fecha si tienes ese campo
                .Take(8)
                .ToList();

            // Obtener estadísticas
            var cotizaciones = await _cotizacionService.GetAllCotizacionesAsync(token);

            var viewModel = new DashboardViewModel
            {
                ProductosRecientes = productosRecientes,
                TotalProductos = productos.Count,
                TotalCotizaciones = cotizaciones.Count,
                TotalVentas = cotizaciones.Sum(c => c.Total),
                ProductosActivos = productos.Count(p => p.Estado),
                ProductosInactivos = productos.Count(p => !p.Estado)
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el dashboard");
            // En caso de error, mostrar dashboard vacío
            return View(new DashboardViewModel());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<string> GetTokenAsync()
    {
        try
        {
            var token = HttpContext.User.FindFirst("Token")?.Value;
            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener token");
            return null;
        }
    }
}

// ViewModel para el dashboard
public class DashboardViewModel
{
    public List<ProductoReadDTO> ProductosRecientes { get; set; } = new();
    public int TotalProductos { get; set; }
    public int TotalCotizaciones { get; set; }
    public decimal TotalVentas { get; set; }
    public int ProductosActivos { get; set; }
    public int ProductosInactivos { get; set; }
}