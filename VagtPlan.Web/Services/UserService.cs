namespace VagtPlan.Web.Services
{
    using System.Net.Http.Json;

    public class UserService
    {
        private readonly HttpClient _http;
        private const string BasePath = "api/User";

        public UserService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<UserDto>>($"{BasePath}/get") ?? new List<UserDto>();
        }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
