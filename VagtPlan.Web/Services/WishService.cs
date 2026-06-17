namespace VagtPlan.Web.Services
{
    using System.Net.Http.Json;
    using System.Net.Http.Headers;

    public class WishService
    {
        private readonly HttpClient _http;
        private readonly ApiAuthState _authState;
        private const string BasePath = "api/SpecialWishes";

        public WishService(HttpClient http, ApiAuthState authState)
        {
            _http = http;
            _authState = authState;
        }

        public async Task<List<SpecialWishDto>> GetAllAsync()
        {
            EnsureAuthorizedRequest();
            return await _http.GetFromJsonAsync<List<SpecialWishDto>>($"{BasePath}/get") ?? new List<SpecialWishDto>();
        }

        public async Task<SpecialWishDto?> GetAsync(int id)
        {
            EnsureAuthorizedRequest();
            return await _http.GetFromJsonAsync<SpecialWishDto>($"{BasePath}/get/{id}");
        }

        public async Task<SpecialWishDto?> CreateAsync(SpecialWishRequest request)
        {
            EnsureAuthorizedRequest();
            var resp = await _http.PostAsJsonAsync($"{BasePath}/createSpecialWish", request);
            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new InvalidOperationException("Du er ikke logget ind.");
            }
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<SpecialWishDto>();
        }

        public async Task<bool> UpdateAsync(int id, SpecialWishRequest request)
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
