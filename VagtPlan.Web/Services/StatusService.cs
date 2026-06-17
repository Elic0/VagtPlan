namespace VagtPlan.Web.Services
{
    using System.Net.Http.Json;
    using System.Net.Http.Headers;

    public class StatusService
    {
        private readonly HttpClient _http;
        private readonly ApiAuthState _authState;
        // Base path for status endpoints (relative to HttpClient.BaseAddress)
        private const string BasePath = "api/Status";

        public StatusService(HttpClient http, ApiAuthState authState)
        {
            _http = http;
            _authState = authState;
        }

        public async Task<List<StatusDto>> GetAllAsync()
        {
            EnsureAuthorizedRequest();
            return await _http.GetFromJsonAsync<List<StatusDto>>($"{BasePath}/get") ?? new List<StatusDto>();
        }

        public async Task<StatusDto?> GetAsync(int id)
        {
            EnsureAuthorizedRequest();
            return await _http.GetFromJsonAsync<StatusDto>($"{BasePath}/get/{id}");
        }

        public async Task<StatusDto?> CreateAsync(StatusRequest request)
        {
            EnsureAuthorizedRequest();
            var resp = await _http.PostAsJsonAsync($"{BasePath}/createStatus", request);
            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new InvalidOperationException("Du er ikke logget ind.");
            }
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<StatusDto>();
        }

        public async Task<bool> UpdateAsync(int id, StatusRequest request)
        {
            EnsureAuthorizedRequest();
            var resp = await _http.PutAsJsonAsync($"{BasePath}/edit/{id}", request);
            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new InvalidOperationException("Du er ikke logget ind.");
            }
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            EnsureAuthorizedRequest();
            var resp = await _http.DeleteAsync($"{BasePath}/delete/{id}");
            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new InvalidOperationException("Du er ikke logget ind.");
            }
            return resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.NoContent;
        }

        private void EnsureAuthorizedRequest()
        {
            if (!_authState.IsAuthenticated || string.IsNullOrWhiteSpace(_authState.Token))
            {
                throw new InvalidOperationException("Du er ikke logget ind.");
            }

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authState.Token);
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
