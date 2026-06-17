namespace VagtPlan.Web.Services
{
    using System.Net.Http.Json;
    using System.Net.Http.Headers;

    public class WorkDayService
    {
        private readonly HttpClient _http;
        private readonly ApiAuthState _authState;
        private const string BasePath = "api/Workdays";

        public WorkDayService(HttpClient http, ApiAuthState authState)
        {
            _http = http;
            _authState = authState;
        }

        public async Task<List<WorkDayDto>> GetAllAsync()
        {
            EnsureAuthorizedRequest();
            return await _http.GetFromJsonAsync<List<WorkDayDto>>($"{BasePath}/get") ?? new List<WorkDayDto>();
        }

        public async Task<WorkDayDto?> GetAsync(int id)
        {
            EnsureAuthorizedRequest();
            return await _http.GetFromJsonAsync<WorkDayDto>($"{BasePath}/get/{id}");
        }

        public async Task<WorkDayDto?> CreateAsync(WorkDayRequest request)
        {
            EnsureAuthorizedRequest();
            var resp = await _http.PostAsJsonAsync($"{BasePath}/createWorkDay", request);
            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new InvalidOperationException("Du er ikke logget ind.");
            }
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<WorkDayDto>();
        }

        public async Task<GenerateWorkdaysResult?> GenerateAsync(GenerateWorkdaysRequest request)
        {
            EnsureAuthorizedRequest();
            var resp = await _http.PostAsJsonAsync($"{BasePath}/generate", request);
            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new InvalidOperationException("Du er ikke logget ind.");
            }
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<GenerateWorkdaysResult>();
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

        public async Task<bool> UpdateAsync(int id, WorkDayRequest request)
        {
            EnsureAuthorizedRequest();
            var resp = await _http.PutAsJsonAsync($"{BasePath}/edit/{id}", request);
            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new InvalidOperationException("Du er ikke logget ind.");
            }
            return resp.IsSuccessStatusCode;
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
