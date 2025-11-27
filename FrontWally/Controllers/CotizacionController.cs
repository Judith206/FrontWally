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

        // GET: Filtrar por fecha
        [HttpGet]
        public async Task<IActionResult> PorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var authToken = await GetTokenAsync();

                // Validar fechas
                if (fechaInicio == DateTime.MinValue || fechaFin == DateTime.MinValue)
                {
                    TempData["Error"] = "Por favor selecciona ambas fechas";
                    return RedirectToAction(nameof(Index));
                }

                if (fechaInicio > fechaFin)
                {
                    TempData["Error"] = "La fecha de inicio no puede ser mayor a la fecha fin";
                    return RedirectToAction(nameof(Index));
                }

                var cotizaciones = await _cotizacionService.GetCotizacionesByFechaRangeAsync(fechaInicio, fechaFin, authToken);

                if (!cotizaciones.Any())
                {
                    TempData["Info"] = $"No se encontraron cotizaciones entre {fechaInicio:dd/MM/yyyy} y {fechaFin:dd/MM/yyyy}";
                }
                else
                {
                    TempData["Success"] = $"Se encontraron {cotizaciones.Count} cotizaciones entre {fechaInicio:dd/MM/yyyy} y {fechaFin:dd/MM/yyyy}";
                }

                return View("Index", cotizaciones);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al filtrar por fecha: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Buscar por contacto
        [HttpGet]
        public async Task<IActionResult> Buscar(string termino)
        {
            try
            {
                var authToken = await GetTokenAsync();

                if (string.IsNullOrWhiteSpace(termino))
                {
                    TempData["Error"] = "Por favor ingresa un término de búsqueda";
                    return RedirectToAction(nameof(Index));
                }

                // Obtener todas las cotizaciones y filtrar localmente
                var todasCotizaciones = await _cotizacionService.GetAllCotizacionesAsync(authToken);
                var cotizacionesFiltradas = todasCotizaciones
                    .Where(c => c.Contacto.Contains(termino, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!cotizacionesFiltradas.Any())
                {
                    TempData["Info"] = $"No se encontraron cotizaciones para el contacto: '{termino}'";
                }
                else
                {
                    TempData["Success"] = $"Se encontraron {cotizacionesFiltradas.Count} cotizaciones para: '{termino}'";
                }

                return View("Index", cotizacionesFiltradas);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al buscar cotizaciones: " + ex.Message;
                return RedirectToAction(nameof(Index));
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

        // POST: Crear nueva cotización
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

        // GetTokenAsync
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

        // POST: Cotizacion/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var authToken = await GetTokenAsync();
                var cotizacion = await _cotizacionService.GetCotizacionByIdAsync(id, authToken);

                if (cotizacion == null)
                {
                    TempData["Error"] = "Cotización no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                if (cotizacion.UsuarioId != GetUsuarioId())
                {
                    TempData["Error"] = "No tienes permiso para eliminar esta cotización";
                    return RedirectToAction(nameof(Index));
                }

                await _cotizacionService.DeleteCotizacionAsync(id, authToken);
                TempData["Success"] = "Cotización eliminada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar cotización: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

    }
}