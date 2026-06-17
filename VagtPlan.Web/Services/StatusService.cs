using VagtPlan.Web.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace VagtPlan.Web.Services
{
    public class StatusService(HttpClient httpClient, ApiAuthState authState)
    {
        // Base path for status endpoints (relative to HttpClient.BaseAddress)
        private const string BasePath = "api/Status";

        public async Task<List<StatusDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            return await httpClient.GetFromJsonAsync<List<StatusDto>>($"{BasePath}/get", cancellationToken) ?? new List<StatusDto>();
        }

        public async Task<StatusDto?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            return await httpClient.GetFromJsonAsync<StatusDto>($"{BasePath}/get/{id}", cancellationToken);
        }

        public async Task<StatusDto?> CreateAsync(StatusRequest request, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await httpClient.PostAsJsonAsync($"{BasePath}/createStatus", request, cancellationToken);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<StatusDto>();
        }

        public async Task<bool> UpdateAsync(int id, StatusRequest request, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await httpClient.PutAsJsonAsync($"{BasePath}/edit/{id}", request, cancellationToken);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await httpClient.DeleteAsync($"{BasePath}/delete/{id}", cancellationToken);
            return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent;
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

    // Client-side DTOs for the Blazor app
    public class StatusDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Colour { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public bool Default { get; set; }
    }

    public class StatusRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Colour { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public bool Default { get; set; }
    }
}
