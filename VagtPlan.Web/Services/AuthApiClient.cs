using System.Net.Http.Json;
using VagtPlan.Web.Models;

namespace VagtPlan.Web.Services;

public class AuthApiClient(HttpClient httpClient)
{
    public async Task<LoginResponseModel?> LoginAsync(LoginRequestModel request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/Auth/login", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new HttpRequestException("Login fejlede.");
            }

            throw new HttpRequestException(errorMessage.Trim('"'));
        }

        return await response.Content.ReadFromJsonAsync<LoginResponseModel>(cancellationToken);
    }
}
