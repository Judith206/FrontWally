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
                var authToken = await GetTokenAsync();
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
                // Validar el modelo primero
                //if (!ModelState.IsValid)
                //{
                //    return View(createDto);
                //}

                // Verificar que se haya subido una imagen
                if (createDto.ImagenFile == null || createDto.ImagenFile.Length == 0)
                {
                    ModelState.AddModelError("ImagenFile", "Debes seleccionar una imagen.");
                    return View(createDto);
                }

                // Validar tamaño de la imagen (máximo 5MB)
                if (createDto.ImagenFile.Length > 6 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImagenFile", "La imagen no puede ser mayor a 5MB.");
                    return View(createDto);
                }

                // Validar tipo de archivo
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(createDto.ImagenFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ImagenFile", "Solo se permiten archivos JPG, JPEG, PNG o GIF.");
                    return View(createDto);
                }

                // Convertir la imagen a byte[]
                using (var ms = new MemoryStream())
                {
                    await createDto.ImagenFile.CopyToAsync(ms);
                    createDto.Imagen = ms.ToArray();
                }

                // Asignar el usuario que crea el producto
                createDto.UsuarioId = GetUsuarioId();

                // OBTENER TOKEN REAL (CORREGIDO)
                var authToken = await GetTokenAsync();
                if (string.IsNullOrEmpty(authToken))
                {
                    ViewBag.Error = "No se pudo obtener el token de autenticación";
                    return View(createDto);
                }

                // Llamada al servicio para enviar al API
                var resultado = await _productoService.CreateProductoAsync(createDto, authToken);

                if (resultado != null && resultado.Id > 0)
                {
                    TempData["Success"] = "Producto creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = "Error al crear el producto - No se recibió respuesta del servidor";
                return View(createDto);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al crear el producto: {ex.Message}";
                if (ex.InnerException != null)
                {
                    ViewBag.Error += $" - {ex.InnerException.Message}";
                }
                return View(createDto);
            }
        }

        // CORREGIR: Método para obtener token real
        private async Task<string> GetTokenAsync()
        {
            try
            {
                // Si estás usando autenticación basada en cookies, no necesitas buscar un token específico
                // Puedes obtener los claims del usuario autenticado
                var token = HttpContext.User.FindFirst("Token")?.Value;

                if (string.IsNullOrEmpty(token))
                {
                    // Si no encuentras el "Token" en los claims, podrías intentar obtener el JWT del encabezado Authorization
                    var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                    if (authHeader != null && authHeader.StartsWith("Bearer "))
                    {
                        token = authHeader.Substring("Bearer ".Length).Trim();
                    }
                }

                return token;
            }
            catch
            {
                return null;
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

                var authToken =await GetTokenAsync();
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
                var authToken = await GetTokenAsync();
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
                var authToken = await GetTokenAsync();
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
                var authToken = await GetTokenAsync();
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
                var authToken = await GetTokenAsync();
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


        private int GetUsuarioId()
        {
            var userIdClaim = User.FindFirst("UserId");
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }




        [HttpGet("TestData")]
        public IActionResult TestData()
        {
            // Datos de prueba PARA VERIFICAR que la vista funciona
            var testProductos = new List<ProductoReadDTO>
    {
        new ProductoReadDTO
        {
            Id = 1,
            Nombre = "PRODUCTO TEST 1",
            Descripcion = "Esta es una descripción de prueba 1",
            Estado = true,
            Precio = 99.99m,
            UsuarioId = 4,
            Imagen = null
        },
        new ProductoReadDTO
        {
            Id = 2,
            Nombre = "PRODUCTO TEST 2",
            Descripcion = "Esta es una descripción de prueba 2",
            Estado = false,
            Precio = 149.50m,
            UsuarioId = 4,
            Imagen = null
        },
        new ProductoReadDTO
        {
            Id = 3,
            Nombre = "PRODUCTO TEST 3",
            Descripcion = "Esta es una descripción de prueba 3",
            Estado = true,
            Precio = 199.99m,
            UsuarioId = 4,
            Imagen = null
        }
    };

            return View("Index", testProductos);
        }
    }
}
