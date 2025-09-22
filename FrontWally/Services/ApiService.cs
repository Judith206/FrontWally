using System.Text.Json;

namespace FrontWally.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        //Obtener Todos
        public async Task<List<T>> GetAllAsync<T>(string endpoint, string token = null)
        {
            AddAuthorizationHeader(token);
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<T>>(json, _jsonOptions);
        }

        //Obtener por Id
        public async Task<T> GetByIdAsync<T>(string endpoint, int id, string token = null)
        {
            AddAuthorizationHeader(token);
            var response = await _httpClient.GetAsync($"{endpoint}/{id}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
    }
}
