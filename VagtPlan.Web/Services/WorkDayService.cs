namespace VagtPlan.Web.Services
{
    using System.Net.Http.Json;

    public class WorkDayService
    {
        private readonly HttpClient _http;
        private const string BasePath = "api/Workdays";

        public WorkDayService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<WorkDayDto>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<WorkDayDto>>($"{BasePath}/get") ?? new List<WorkDayDto>();
        }

        public async Task<WorkDayDto?> GetAsync(int id)
        {
            return await _http.GetFromJsonAsync<WorkDayDto>($"{BasePath}/get/{id}");
        }

        public async Task<WorkDayDto?> CreateAsync(WorkDayRequest request)
        {
            var resp = await _http.PostAsJsonAsync($"{BasePath}/createWorkDay", request);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<WorkDayDto>();
        }

        public async Task<GenerateWorkdaysResult?> GenerateAsync(GenerateWorkdaysRequest request)
        {
            var resp = await _http.PostAsJsonAsync($"{BasePath}/generate", request);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<GenerateWorkdaysResult>();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var resp = await _http.DeleteAsync($"{BasePath}/delete/{id}");
            return resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.NoContent;
        }

        public async Task<bool> UpdateAsync(int id, WorkDayRequest request)
        {
            var resp = await _http.PutAsJsonAsync($"{BasePath}/edit/{id}", request);
            return resp.IsSuccessStatusCode;
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
