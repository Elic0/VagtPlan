using VagtPlan.Web.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace VagtPlan.Web.Services
{
    public class UserApiClient(HttpClient httpClient, ApiAuthState authState)
    {

        public async Task<UserModel[]> GetAllAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var users = await httpClient.GetFromJsonAsync<UserModel[]>("api/User/get", cancellationToken);
            return users ?? Array.Empty<UserModel>();
        }

        public async Task<UserModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            return await httpClient.GetFromJsonAsync<UserModel>($"api/User/get/{id}", cancellationToken);
        }

        public async Task<UserModel?> CreateAsync(UserDto dto, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await httpClient.PostAsJsonAsync("api/User/createUser", dto, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserModel>(cancellationToken);
        }

        public async Task<UserModel?> UpdateAsync(int id, UserDto dto, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await httpClient.PutAsJsonAsync($"api/User/edit/{id}", dto, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserModel>(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await httpClient.DeleteAsync($"api/User/delete/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await httpClient.PostAsJsonAsync("api/User/login", dto, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        private void EnsureAuthorizedRequest()
        {
            if (!authState.IsAuthenticated || string.IsNullOrWhiteSpace(authState.Token))
            {
                throw new InvalidOperationException("Du er ikke logget ind.");
            }

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authState.Token);
        }
    }
}
