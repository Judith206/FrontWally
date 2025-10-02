using FrontWally.DTOs.CotizacionDTOs;
using FrontWally.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrontWally.Controllers
{
    [Authorize]
    public class CotizacionController : Controller
    {
        private readonly CotizacionService _cotizacionService;
        private readonly ProductoService _productoService;

        public CotizacionController(CotizacionService cotizacionService, ProductoService productoService)
        {
            _cotizacionService = cotizacionService;
            _productoService = productoService;
        }

        // GET: Mostrar lista de cotizaciones
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var authToken = await GetTokenAsync();
                var cotizaciones = await _cotizacionService.GetAllCotizacionesAsync(authToken);
                return View(cotizaciones);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar las cotizaciones: " + ex.Message;
                return View(new List<CotizacionDTO>());
            }
        }

        // GET: Mostrar formulario de creación
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            try
            {
                var authToken = await GetTokenAsync();
                var productos = await _productoService.GetAllProductosAsync(authToken);
                ViewBag.Productos = productos;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar los productos: " + ex.Message;
                return View();
            }
        }

        // POST: Crear nueva cotización - CORREGIDO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CotizacionCreateDTO createDto)
        {
            try
            {
                Console.WriteLine(" CotizacionController.Crear POST - Iniciando...");

                if (!ModelState.IsValid)
                {
                    Console.WriteLine(" ModelState no es válido");
                    var authToken = await GetTokenAsync();
                    var productos = await _productoService.GetAllProductosAsync(authToken);
                    ViewBag.Productos = productos;
                    return View(createDto);
                }

                // Asignar el usuario que crea la cotización
                createDto.UsuarioId = GetUsuarioId();

                // CALCULAR EL TOTAL BASADO EN EL PRODUCTO SELECCIONADO
                Console.WriteLine(" Calculando total...");
                var authTokenForProducto = await GetTokenAsync();
                var producto = await _productoService.GetProductoByIdAsync(createDto.ProductoId, authTokenForProducto);

                if (producto != null)
                {
                    // Calcular el total: Cantidad × Precio del Producto
                    createDto.Total = createDto.Cantidad * producto.Precio;
                    Console.WriteLine($" Total calculado: {createDto.Cantidad} × {producto.Precio:C} = {createDto.Total:C}");
                }
                else
                {
                    Console.WriteLine(" No se pudo obtener el producto para calcular el total");
                    ViewBag.Error = "Error al calcular el total - Producto no encontrado";
                    var productosList = await _productoService.GetAllProductosAsync(authTokenForProducto);
                    ViewBag.Productos = productosList;
                    return View(createDto);
                }

                Console.WriteLine($" Datos completos de cotización:");
                Console.WriteLine($"   - Contacto: {createDto.Contacto}");
                Console.WriteLine($"   - Fecha: {createDto.Fecha}");
                Console.WriteLine($"   - ProductoId: {createDto.ProductoId}");
                Console.WriteLine($"   - Cantidad: {createDto.Cantidad}");
                Console.WriteLine($"   - Total: {createDto.Total:C}");
                Console.WriteLine($"   - UsuarioId: {createDto.UsuarioId}");

                var authToken2 = await GetTokenAsync();
                Console.WriteLine($" Token para crear: {!string.IsNullOrEmpty(authToken2)}");

                Console.WriteLine(" Llamando a CotizacionService.CreateCotizacionAsync...");
                var resultado = await _cotizacionService.CreateCotizacionAsync(createDto, authToken2);

                if (resultado != null && resultado.Id > 0)
                {
                    Console.WriteLine($" Cotización creada exitosamente con ID: {resultado.Id}");
                    TempData["Success"] = "Cotización creada exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    Console.WriteLine(" No se pudo crear la cotización - resultado nulo o ID inválido");
                    ViewBag.Error = "Error al crear la cotización - No se recibió respuesta del servidor";
                    var authToken3 = await GetTokenAsync();
                    var productosList = await _productoService.GetAllProductosAsync(authToken3);
                    ViewBag.Productos = productosList;
                    return View(createDto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" EXCEPCIÓN en Crear POST: {ex.Message}");
                Console.WriteLine($" StackTrace: {ex.StackTrace}");

                ViewBag.Error = "Error al crear la cotización: " + ex.Message;
                var authToken4 = await GetTokenAsync();
                var productos = await _productoService.GetAllProductosAsync(authToken4);
                ViewBag.Productos = productos;
                return View(createDto);
            }
        }

        // ... (otros métodos mantienen la misma estructura con await GetTokenAsync())

        // ✅ MÉTODO GetTokenAsync CORREGIDO
        private async Task<string> GetTokenAsync()
        {
            try
            {
                var token = HttpContext.User.FindFirst("Token")?.Value;

                if (string.IsNullOrEmpty(token))
                {
                    var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                    if (authHeader != null && authHeader.StartsWith("Bearer "))
                    {
                        token = authHeader.Substring("Bearer ".Length).Trim();
                    }
                }

                Console.WriteLine($" Token obtenido: {!string.IsNullOrEmpty(token)}");
                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en GetTokenAsync: {ex.Message}");
                return null;
            }
        }

        private int GetUsuarioId()
        {
            var userIdClaim = User.FindFirst("UserId");
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}