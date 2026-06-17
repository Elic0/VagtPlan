using VagtPlan.Web.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace VagtPlan.Web.Services
{
    public class WishService(HttpClient httpClient, ApiAuthState authState)
    {
        private const string BasePath = "api/SpecialWishes";


        public async Task<List<SpecialWishDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            return await httpClient.GetFromJsonAsync<List<SpecialWishDto>>($"{BasePath}/get", cancellationToken) ?? new List<SpecialWishDto>();
        }

        public async Task<SpecialWishDto?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            return await httpClient.GetFromJsonAsync<SpecialWishDto>($"{BasePath}/get/{id}", cancellationToken);
        }

        public async Task<SpecialWishDto?> CreateAsync(SpecialWishRequest request, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await httpClient.PostAsJsonAsync($"{BasePath}/createSpecialWish", request, cancellationToken);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<SpecialWishDto>();
        }

        public async Task<bool> UpdateAsync(int id, SpecialWishRequest request, CancellationToken cancellationToken = default)
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
