using FrontWally.DTOs.ProductoDTOs;
using FrontWally.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrontWally.Controllers
{
    [Authorize]
    public class ProductoController : Controller
    {
        private readonly ProductoService _productoService;

        public ProductoController(ProductoService productoService)
        {
            _productoService = productoService;
        }

        // GET: Mostrar lista de productos
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var authToken = GetToken();
                var productos = await _productoService.GetAllProductosAsync(authToken);
                return View(productos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar los productos: " + ex.Message;
                return View(new List<ProductoReadDTO>());
            }
        }

        // GET: Mostrar formulario de creación
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Crear nuevo producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ProductoCreateDTO createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(createDto);
                }

                // Convertir la imagen a byte[]
                if (createDto.ImagenFile != null && createDto.ImagenFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await createDto.ImagenFile.CopyToAsync(ms);
                        createDto.Imagen = ms.ToArray();
                    }
                }
                else
                {
                    ModelState.AddModelError("ImagenFile", "Debes seleccionar una imagen.");
                    return View(createDto);
                }

                var authToken = GetToken();
                var resultado = await _productoService.CreateProductoAsync(createDto, authToken);

                if (resultado != null && resultado.IdProducto > 0)
                {
                    TempData["Success"] = "Producto creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = "Error al crear el producto";
                return View(createDto);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al crear el producto: " + ex.Message;
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
                var producto = await _productoService.GetProductoByIdAsync(id, authToken);

                if (producto == null)
                {
                    TempData["Error"] = "Producto no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                var updateDto = new ProductoUpdateDTO
                {
                    IdProducto = producto.IdProducto,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Estado = producto.Estado,
                    Precio = producto.Precio,
                    Imagen = producto.Imagen
                };

                return View(updateDto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el producto: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Actualizar producto
        [HttpPost("Editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, ProductoUpdateDTO updateDto)
        {
            try
            {
                if (id != updateDto.IdProducto)
                {
                    TempData["Error"] = "ID del producto no coincide";
                    return RedirectToAction(nameof(Index));
                }

                if (!ModelState.IsValid)
                {
                    return View(updateDto);
                }

                // Si sube nueva imagen, reemplazar
                if (updateDto.ImagenFile != null && updateDto.ImagenFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await updateDto.ImagenFile.CopyToAsync(ms);
                        updateDto.Imagen = ms.ToArray();
                    }
                }

                var authToken = GetToken();
                var resultado = await _productoService.UpdateProductoAsync(updateDto, authToken);

                if (resultado != null)
                {
                    TempData["Success"] = "Producto actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = "Error al actualizar el producto";
                return View(updateDto);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar el producto: " + ex.Message;
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
                var producto = await _productoService.GetProductoByIdAsync(id, authToken);

                if (producto == null)
                {
                    TempData["Error"] = "Producto no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el producto: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Eliminar producto
        [HttpPost("Eliminar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                var authToken = GetToken();
                var resultado = await _productoService.DeleteProductoAsync(id, authToken);

                if (resultado)
                    TempData["Success"] = "Producto eliminado exitosamente";
                else
                    TempData["Error"] = "Error al eliminar el producto";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el producto: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Buscar productos
        [HttpGet("Buscar")]
        public async Task<IActionResult> Buscar(string termino)
        {
            try
            {
                var authToken = GetToken();
                List<ProductoReadDTO> productos;

                if (string.IsNullOrEmpty(termino))
                    productos = await _productoService.GetAllProductosAsync(authToken);
                else
                    productos = await _productoService.SearchProductosAsync(termino, authToken);

                return View("Index", productos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al buscar productos: " + ex.Message;
                return View("Index", new List<ProductoReadDTO>());
            }
        }

        // GET: Productos por usuario
        [HttpGet("MisProductos")]
        public async Task<IActionResult> MisProductos()
        {
            try
            {
                var authToken = GetToken();
                var usuarioId = GetUsuarioId();
                var productos = await _productoService.GetProductosByUsuarioAsync(usuarioId, authToken);

                return View("Index", productos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar tus productos: " + ex.Message;
                return View("Index", new List<ProductoReadDTO>());
            }
        }

        private string GetToken() => string.Empty;

        private int GetUsuarioId()
        {
            var userIdClaim = User.FindFirst("UserId");
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
