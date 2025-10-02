using FrontWally.DTOs.ProductoDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrontWally.Services
{
    public class ProductoService
    {
        private readonly ApiService _apiService;

        public ProductoService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Crear un nuevo producto
        // Crear un nuevo producto (AGREGAR LOGGING PARA DEBUG)
        public async Task<ProductoReadDTO> CreateProductoAsync(ProductoCreateDTO createDto, string token = null)
        {
            try
            {
                Console.WriteLine($"Enviando producto: {createDto.Nombre}");
                Console.WriteLine($"Imagen tamaño: {createDto.Imagen?.Length ?? 0} bytes");

                return await _apiService.PostAsync<ProductoCreateDTO, ProductoReadDTO>("producto", createDto, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateProductoAsync: {ex.Message}");
                throw;
            }
        }

        // Obtener todos los productos
        public async Task<List<ProductoReadDTO>> GetAllProductosAsync(string token = null)
        {
            return await _apiService.GetAllAsync<ProductoReadDTO>("Producto/productos", token);
        }

        // Obtener producto por ID (versión simple)
        public async Task<ProductoReadDTO> GetProductoByIdAsync(int id, string token = null)
        {
            return await _apiService.GetByIdAsync<ProductoReadDTO>("producto", id, token);
        }

        // Obtener detalles completos del producto (con usuario y cotizaciones)
        public async Task<ProductoDetailDTO> GetProductoDetailAsync(int id, string token = null)
        {
            return await _apiService.GetByIdAsync<ProductoDetailDTO>("producto/detail", id, token);
        }

        // Obtener productos por usuario
        public async Task<List<ProductoReadDTO>> GetProductosByUsuarioAsync(int usuarioId, string token = null)
        {
            return await _apiService.GetAllAsync<ProductoReadDTO>($"producto/usuario/{usuarioId}", token);
        }

        // Obtener productos por estado
        public async Task<List<ProductoReadDTO>> GetProductosByEstadoAsync(string estado, string token = null)
        {
            return await _apiService.GetAllAsync<ProductoReadDTO>($"producto/estado/{estado}", token);
        }

        // Actualizar producto
        public async Task<ProductoReadDTO> UpdateProductoAsync(ProductoUpdateDTO updateDto, string token = null)
        {
            return await _apiService.PutAsync<ProductoUpdateDTO, ProductoReadDTO>("producto", updateDto.Id, updateDto, token);
        }

        // Eliminar producto
        public async Task<bool> DeleteProductoAsync(int id, string token = null)
        {
            return await _apiService.DeleteAsync("producto", id, token);
        }

        // Buscar productos por nombre (búsqueda parcial)
        public async Task<List<ProductoReadDTO>> SearchProductosAsync(string searchTerm, string token = null)
        {
            return await _apiService.GetAllAsync<ProductoReadDTO>($"producto/search/{searchTerm}", token);
        }

        // Obtener productos por rango de precio
        public async Task<List<ProductoReadDTO>> GetProductosByPrecioRangeAsync(decimal precioMin, decimal precioMax, string token = null)
        {
            return await _apiService.GetAllAsync<ProductoReadDTO>($"producto/precio?precioMin={precioMin}&precioMax={precioMax}", token);
        }

        // Obtener productos activos
        public async Task<List<ProductoReadDTO>> GetProductosActivosAsync(string token = null)
        {
            return await _apiService.GetAllAsync<ProductoReadDTO>("producto/activos", token);
        }
    }
}