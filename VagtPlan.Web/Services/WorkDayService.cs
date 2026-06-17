using VagtPlan.Web.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace VagtPlan.Web.Services
{
    public class WorkDayService(HttpClient httpClient, ApiAuthState authState)
    {
        private const string BasePath = "api/Workdays";

        public async Task<List<WorkDayDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            return await httpClient.GetFromJsonAsync<List<WorkDayDto>>($"{BasePath}/get", cancellationToken) ?? new List<WorkDayDto>();
        }

        public async Task<WorkDayDto?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            return await httpClient.GetFromJsonAsync<WorkDayDto>($"{BasePath}/get/{id}", cancellationToken);
        }

        public async Task<WorkDayDto?> CreateAsync(WorkDayRequest request, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await httpClient.PostAsJsonAsync($"{BasePath}/createWorkDay", request, cancellationToken);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<WorkDayDto>();
        }

        public async Task<GenerateWorkdaysResult?> GenerateAsync(GenerateWorkdaysRequest request, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await httpClient.PostAsJsonAsync($"{BasePath}/generate", request, cancellationToken);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<GenerateWorkdaysResult>();
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await httpClient.DeleteAsync($"{BasePath}/delete/{id}", cancellationToken);
            return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent;
        }

        public async Task<bool> UpdateAsync(int id, WorkDayRequest request, CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            var response = await httpClient.PutAsJsonAsync($"{BasePath}/edit/{id}", request, cancellationToken);
            return response.IsSuccessStatusCode;
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

    public class WorkDayDto
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int StatusId { get; set; }
        public int UserId { get; set; }
    }

    public class WorkDayRequest
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int StatusId { get; set; }
        public int UserId { get; set; }
    }

    public class GenerateWorkdaysRequest
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public List<int> SelectedUserIds { get; set; } = new();
        public int DepartmentId { get; set; }
    }

    public class GenerateWorkdaysResult
    {
        public List<WorkDayDto> Created { get; set; } = new();
        public Dictionary<int,int> AssignedCounts { get; set; } = new();
    }
}
