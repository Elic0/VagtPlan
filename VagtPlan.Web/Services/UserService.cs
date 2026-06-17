using VagtPlan.Web.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace VagtPlan.Web.Services
{

    public class UserService (HttpClient httpClient, ApiAuthState authState)
    {
        private const string BasePath = "api/User";

        public async Task<List<UserRequestDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthorizedRequest();
            return await httpClient.GetFromJsonAsync<List<UserRequestDto>>($"{BasePath}/get", cancellationToken) ?? new List<UserRequestDto>();
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
}
