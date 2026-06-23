using System.Net.Http.Json;
using System.Net.Http.Headers;
using VagtPlan.Web.Models;

namespace VagtPlan.Web.Services
{
    public class UserApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ApiAuthState _authState;

        public UserApiClient(HttpClient httpClient, ApiAuthState authState)
        {
            _httpClient = httpClient;
            _authState = authState;
        }

        public async Task<UserModel[]> GetAllAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var users = await _httpClient.GetFromJsonAsync<UserModel[]>("api/User/get", cancellationToken);
            return users ?? Array.Empty<UserModel>();
        }

        public async Task<UserModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            return await _httpClient.GetFromJsonAsync<UserModel>($"api/User/get/{id}", cancellationToken);
        }

        public async Task<UserModel?> CreateAsync(UserDto dto, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await _httpClient.PostAsJsonAsync("api/User/createUser", dto, cancellationToken);
            await EnsureSuccessOrThrowAsync(response, cancellationToken);
            return await response.Content.ReadFromJsonAsync<UserModel>(cancellationToken);
        }

        public async Task UpdateAsync(int id, UserDto dto, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await _httpClient.PutAsJsonAsync($"api/User/edit/{id}", dto, cancellationToken);
            await EnsureSuccessOrThrowAsync(response, cancellationToken);
        }

        private static async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var message = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(string.IsNullOrWhiteSpace(message)
                ? "Kunne ikke gemme bruger."
                : message.Trim('"'));
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await _httpClient.DeleteAsync($"api/User/delete/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        private void EnsureAuthorizedRequest()
        {
            if (!_authState.IsAuthenticated || string.IsNullOrWhiteSpace(_authState.Token))
            {
                throw new InvalidOperationException("Du er ikke logget ind.");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authState.Token);
        }
    }
}
