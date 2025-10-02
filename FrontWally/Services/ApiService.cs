using System.Text.Json;
using System.Text.Encodings;
using System.Net.Http.Headers;
using System.Text;
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

        // Post generico
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, string token = null)
        {
            AddAuthorizationHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(data, _jsonOptions), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
        }

        //PUT generico
        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, int id, TRequest data, string token = null)
        {
            AddAuthorizationHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(data, _jsonOptions), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{endpoint}/{id}", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
        }

        //Delete generico
        public async Task<bool> DeleteAsync(string endpoint, int id, string token = null)
        {
            AddAuthorizationHeader(token);
            var response = await _httpClient.DeleteAsync($"{endpoint}/{id}");
            return response.IsSuccessStatusCode;
        }

        //Agregar authorization Header
        private void AddAuthorizationHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
