namespace VagtPlan.Web.Services
{
    using System.Net.Http.Json;
    using System.Net.Http.Headers;

    public class UserService
    {
        private readonly HttpClient _http;
        private readonly ApiAuthState _authState;
        private const string BasePath = "api/User";

        public UserService(HttpClient http, ApiAuthState authState)
        {
            _http = http;
            _authState = authState;
        }

        public async Task<List<UserListItemDto>> GetAllAsync()
        {
            EnsureAuthorizedRequest();
            return await _http.GetFromJsonAsync<List<UserListItemDto>>($"{BasePath}/get") ?? new List<UserListItemDto>();
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

    public class UserListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
    }
}
