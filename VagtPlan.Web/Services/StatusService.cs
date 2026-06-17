namespace VagtPlan.Web.Services
{
    using System.Net.Http.Json;

    public class StatusService
    {
        private readonly HttpClient _http;
        // Base path for status endpoints (relative to HttpClient.BaseAddress)
        private const string BasePath = "api/Status";

        public StatusService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<StatusDto>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<StatusDto>>($"{BasePath}/get") ?? new List<StatusDto>();
        }

        public async Task<StatusDto?> GetAsync(int id)
        {
            return await _http.GetFromJsonAsync<StatusDto>($"{BasePath}/get/{id}");
        }

        public async Task<StatusDto?> CreateAsync(StatusRequest request)
        {
            var resp = await _http.PostAsJsonAsync($"{BasePath}/createStatus", request);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<StatusDto>();
        }

        public async Task<bool> UpdateAsync(int id, StatusRequest request)
        {
            var resp = await _http.PutAsJsonAsync($"{BasePath}/edit/{id}", request);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var resp = await _http.DeleteAsync($"{BasePath}/delete/{id}");
            return resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.NoContent;
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
