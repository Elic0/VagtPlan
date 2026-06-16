using System.Net.Http.Json;
using System.Net.Http.Headers;
using VagtPlan.Web.Models;

namespace VagtPlan.Web.Services;

public class DepartmentApiClient(HttpClient httpClient, ApiAuthState authState)
{
    public async Task<DepartmentModel[]> GetAllAsync(CancellationToken cancellationToken = default)
    {
        EnsureAuthorizedRequest();
        var departments = await httpClient.GetFromJsonAsync<DepartmentModel[]>("api/Department/get", cancellationToken);
        return departments ?? [];
    }

    public async Task<DepartmentModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        EnsureAuthorizedRequest();
        return await httpClient.GetFromJsonAsync<DepartmentModel>($"api/Department/get/{id}", cancellationToken);
    }

    public async Task<DepartmentModel?> CreateAsync(DepartmentDto dto, CancellationToken cancellationToken = default)
    {
        EnsureAuthorizedRequest();
        var response = await httpClient.PostAsJsonAsync("api/Department/createDepartment", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DepartmentModel>(cancellationToken);
    }

    public async Task<DepartmentModel?> UpdateAsync(int id, DepartmentDto dto, CancellationToken cancellationToken = default)
    {
        EnsureAuthorizedRequest();
        var response = await httpClient.PutAsJsonAsync($"api/Department/edit/{id}", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DepartmentModel>(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        EnsureAuthorizedRequest();
        var response = await httpClient.DeleteAsync($"api/Department/delete/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
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
