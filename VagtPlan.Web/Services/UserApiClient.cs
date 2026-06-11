using System.Net.Http.Json;
using VagtPlan.Web.Models;

namespace VagtPlan.Web.Services
{
    public class UserApiClient
    {
        private readonly HttpClient _httpClient;

        public UserApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserModel[]> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await _httpClient.GetFromJsonAsync<UserModel[]>("api/User/get", cancellationToken);
            return users ?? Array.Empty<UserModel>();
        }

        public async Task<UserModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<UserModel>($"api/User/get/{id}", cancellationToken);
        }

        public async Task<UserModel?> CreateAsync(UserDto dto, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/User/createUser", dto, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserModel>(cancellationToken);
        }

        public async Task<UserModel?> UpdateAsync(int id, UserDto dto, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/User/edit/{id}", dto, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserModel>(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"api/User/delete/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/User/login", dto, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
