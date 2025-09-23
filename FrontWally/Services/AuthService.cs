using FrontWally.DTOs.UsuarioDTOs;

namespace FrontWally.Services
{
    public class AuthService
    {
        private readonly ApiService _apiService;

        public AuthService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<LoginResponseDTO?> LoginAsync(UsuarioLoginDTO dto)
        {
            return await _apiService.PostAsync<UsuarioLoginDTO, LoginResponseDTO>("Auth/login", dto);
        }

        public async Task<LoginResponseDTO?> RegistrarAsync(UsuarioRegistroDTO dto)
        {
            try
            {
                return await _apiService.PostAsync<UsuarioRegistroDTO, LoginResponseDTO>("Auth/registrar", dto);
            }
            catch (HttpRequestException ex)
            {
                // Puedes loguear el error si tienes un sistema de logs
                Console.WriteLine($"Error al registrar: {ex.Message}");
                return null;
            }
        }
    }
}
