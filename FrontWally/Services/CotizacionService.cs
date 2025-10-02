using FrontWally.DTOs.CotizacionDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrontWally.Services
{
    public class CotizacionService
    {
        private readonly ApiService _apiService;

        public CotizacionService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Crear una nueva cotización
        public async Task<CotizacionDTO> CreateCotizacionAsync(CotizacionCreateDTO createDto, string token = null)
        {
            return await _apiService.PostAsync<CotizacionCreateDTO, CotizacionDTO>("cotizaciones", createDto, token);
        }

        // Obtener todas las cotizaciones
        public async Task<List<CotizacionDTO>> GetAllCotizacionesAsync(string token = null)
        {
            return await _apiService.GetAllAsync<CotizacionDTO>("cotizaciones", token);
        }

        // Obtener cotización por ID
        public async Task<CotizacionDTO> GetCotizacionByIdAsync(int id, string token = null)
        {
            return await _apiService.GetByIdAsync<CotizacionDTO>("cotizaciones", id, token);
        }

        // Obtener cotizaciones por Usuario
        public async Task<List<CotizacionDTO>> GetCotizacionesByUsuarioAsync(int usuarioId, string token = null)
        {
            return await _apiService.GetAllAsync<CotizacionDTO>($"cotizaciones/usuario/{usuarioId}", token);
        }

        // Obtener cotizaciones por Producto
        public async Task<List<CotizacionDTO>> GetCotizacionesByProductoAsync(int productoId, string token = null)
        {
            return await _apiService.GetAllAsync<CotizacionDTO>($"cotizaciones/producto/{productoId}", token);
        }

        // Actualizar cotización
        public async Task<CotizacionDTO> UpdateCotizacionAsync(int id, CotizacionCreateDTO updateDto, string token = null)
        {
            return await _apiService.PutAsync<CotizacionCreateDTO, CotizacionDTO>("cotizaciones", id, updateDto, token);
        }

        // Eliminar cotización
        public async Task<bool> DeleteCotizacionAsync(int id, string token = null)
        {
            return await _apiService.DeleteAsync("cotizaciones", id, token);
        }

        // Método adicional: Obtener cotizaciones por rango de fechas
        public async Task<List<CotizacionDTO>> GetCotizacionesByFechaRangeAsync(DateTime fechaInicio, DateTime fechaFin, string token = null)
        {
            return await _apiService.GetAllAsync<CotizacionDTO>($"cotizaciones/fechas?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}", token);
        }
    }
}