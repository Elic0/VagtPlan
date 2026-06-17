namespace VagtPlan.Web.Services
{
    using System.Net.Http.Json;

    public class WishService
    {
        private readonly HttpClient _http;
        private const string BasePath = "api/SpecialWishes";

        public WishService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<SpecialWishDto>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<SpecialWishDto>>($"{BasePath}/get") ?? new List<SpecialWishDto>();
        }

        public async Task<SpecialWishDto?> GetAsync(int id)
        {
            return await _http.GetFromJsonAsync<SpecialWishDto>($"{BasePath}/get/{id}");
        }

        public async Task<SpecialWishDto?> CreateAsync(SpecialWishRequest request)
        {
            var resp = await _http.PostAsJsonAsync($"{BasePath}/createSpecialWish", request);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<SpecialWishDto>();
        }

        public async Task<bool> UpdateAsync(int id, SpecialWishRequest request)
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

    public class SpecialWishDto
    {
        public int Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public int UserId { get; set; }
    }

    public class SpecialWishRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public int UserId { get; set; }
    }
}
