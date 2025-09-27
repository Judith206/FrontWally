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
                var authToken = GetToken();
                var cotizaciones = await _cotizacionService.GetAllCotizacionesAsync(authToken);
                return View(cotizaciones);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar las cotizaciones: " + ex.Message;
                return View(new List<CotizacionDTO>());
            }
        }

        // GET: Mostrar detalles de una cotización
        [HttpGet("Detalles/{id}")]
        public async Task<IActionResult> Detalles(int id)
        {
            try
            {
                var authToken = GetToken();
                var cotizacion = await _cotizacionService.GetCotizacionByIdAsync(id, authToken);

                if (cotizacion == null)
                {
                    TempData["Error"] = "Cotización no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                return View(cotizacion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la cotización: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Mostrar formulario de creación
        [HttpGet("Crear")]
        public async Task<IActionResult> Crear()
        {
            try
            {
                var authToken = GetToken();
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
        [HttpPost("Crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CotizacionCreateDTO createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var authToken = GetToken();
                    var productos = await _productoService.GetAllProductosAsync(authToken);
                    ViewBag.Productos = productos;
                    return View(createDto);
                }

                var authToken2 = GetToken();
                var resultado = await _cotizacionService.CreateCotizacionAsync(createDto, authToken2);

                if (resultado != null && resultado.Id > 0)
                {
                    TempData["Success"] = "Cotización creada exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = "Error al crear la cotización";
                var authToken3 = GetToken();
                var productosList = await _productoService.GetAllProductosAsync(authToken3);
                ViewBag.Productos = productosList;
                return View(createDto);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al crear la cotización: " + ex.Message;
                var authToken4 = GetToken();
                var productos = await _productoService.GetAllProductosAsync(authToken4);
                ViewBag.Productos = productos;
                return View(createDto);
            }
        }

        // GET: Mostrar formulario de edición
        [HttpGet("Editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                var authToken = GetToken();
                var cotizacion = await _cotizacionService.GetCotizacionByIdAsync(id, authToken);
                var productos = await _productoService.GetAllProductosAsync(authToken);

                if (cotizacion == null)
                {
                    TempData["Error"] = "Cotización no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                var createDto = new CotizacionCreateDTO
                {
                    Fecha = cotizacion.Fecha,
                    Contacto = cotizacion.Contacto,
                    UsuarioId = cotizacion.UsuarioId,
                    ProductoId = cotizacion.ProductoId
                };

                ViewBag.Productos = productos;
                ViewBag.CotizacionId = id;

                return View(createDto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la cotización: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Actualizar cotización
        [HttpPost("Editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, CotizacionCreateDTO updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var authToken = GetToken();
                    var productos = await _productoService.GetAllProductosAsync(authToken);
                    ViewBag.Productos = productos;
                    ViewBag.CotizacionId = id;
                    return View(updateDto);
                }

                var authToken2 = GetToken();
                var resultado = await _cotizacionService.UpdateCotizacionAsync(id, updateDto, authToken2);

                if (resultado != null)
                {
                    TempData["Success"] = "Cotización actualizada exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = "Error al actualizar la cotización";
                var authToken3 = GetToken();
                var productosList = await _productoService.GetAllProductosAsync(authToken3);
                ViewBag.Productos = productosList;
                ViewBag.CotizacionId = id;
                return View(updateDto);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar la cotización: " + ex.Message;
                var authToken4 = GetToken();
                var productos = await _productoService.GetAllProductosAsync(authToken4);
                ViewBag.Productos = productos;
                ViewBag.CotizacionId = id;
                return View(updateDto);
            }
        }

        // GET: Mostrar confirmación de eliminación
        [HttpGet("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var authToken = GetToken();
                var cotizacion = await _cotizacionService.GetCotizacionByIdAsync(id, authToken);

                if (cotizacion == null)
                {
                    TempData["Error"] = "Cotización no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                return View(cotizacion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la cotización: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Eliminar cotización
        [HttpPost("Eliminar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                var authToken = GetToken();
                var resultado = await _cotizacionService.DeleteCotizacionAsync(id, authToken);

                if (resultado)
                {
                    TempData["Success"] = "Cotización eliminada exitosamente";
                }
                else
                {
                    TempData["Error"] = "Error al eliminar la cotización";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar la cotización: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Mis cotizaciones (por usuario)
        [HttpGet("MisCotizaciones")]
        public async Task<IActionResult> MisCotizaciones()
        {
            try
            {
                var authToken = GetToken();
                var usuarioId = GetUsuarioId();
                var cotizaciones = await _cotizacionService.GetCotizacionesByUsuarioAsync(usuarioId, authToken);

                return View("Index", cotizaciones);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar tus cotizaciones: " + ex.Message;
                return View("Index", new List<CotizacionDTO>());
            }
        }

        // GET: Cotizaciones por producto
        [HttpGet("PorProducto/{productoId}")]
        public async Task<IActionResult> PorProducto(int productoId)
        {
            try
            {
                var authToken = GetToken();
                var cotizaciones = await _cotizacionService.GetCotizacionesByProductoAsync(productoId, authToken);

                ViewBag.ProductoId = productoId;
                return View("Index", cotizaciones);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar las cotizaciones: " + ex.Message;
                return View("Index", new List<CotizacionDTO>());
            }
        }

        // GET: Cotizaciones por rango de fechas
        [HttpGet("PorFecha")]
        public async Task<IActionResult> PorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var authToken = GetToken();
                var cotizaciones = await _cotizacionService.GetCotizacionesByFechaRangeAsync(fechaInicio, fechaFin, authToken);

                ViewBag.FechaInicio = fechaInicio;
                ViewBag.FechaFin = fechaFin;
                return View("Index", cotizaciones);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar las cotizaciones: " + ex.Message;
                return View("Index", new List<CotizacionDTO>());
            }
        }

        private string GetToken()
        {
            // Ejemplo: return HttpContext.Request.Cookies["AuthToken"];
            return string.Empty;
        }

        private int GetUsuarioId()
        {
            var userIdClaim = User.FindFirst("UserId");
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}