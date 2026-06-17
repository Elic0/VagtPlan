using VagtPlan.Web.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;


namespace VagtPlan.Web.Services
{
    public class UserRoleApiClient(HttpClient httpClient, ApiAuthState authState)
    {
        public async Task<UserRoleModel[]> GetAllAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var userRoles = await httpClient.GetFromJsonAsync<UserRoleModel[]>("api/UserRole/get", cancellationToken);
            return userRoles ?? [];
        }

        public async Task<UserRoleModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            return await httpClient.GetFromJsonAsync<UserRoleModel>($"api/UserRole/get/{id}", cancellationToken);
        }

        public async Task<UserRoleModel?> CreateAsync(UserRoleDto dto, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await httpClient.PostAsJsonAsync("api/UserRole/createUserRole", dto, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserRoleModel>(cancellationToken);
        }

        public async Task<UserRoleModel?> UpdateAsync(int id, UserRoleDto dto, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await httpClient.PutAsJsonAsync($"api/UserRole/edit/{id}", dto, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserRoleModel>(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await httpClient.DeleteAsync($"api/UserRole/delete/{id}", cancellationToken);
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
