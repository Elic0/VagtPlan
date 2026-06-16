using System.Net.Http.Json;
using System.Net.Http.Headers;
using VagtPlan.Web.Models;

namespace VagtPlan.Web.Services;

public class WorkTimeApiClient(HttpClient httpClient, ApiAuthState authState)
{
    public async Task<WorkTimeModel[]> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        EnsureAuthorizedRequest();
        var workTimes = await httpClient.GetFromJsonAsync<WorkTimeModel[]>(
            $"api/WorkTime/get/byDepartment/{departmentId}",
            cancellationToken);

        return workTimes ?? [];
    }

    public async Task<WorkTimeModel?> CreateAsync(WorkTimeDto dto, CancellationToken cancellationToken = default)
    {
        EnsureAuthorizedRequest();
        var response = await httpClient.PostAsJsonAsync("api/WorkTime/createWorkTime", dto, cancellationToken);
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<WorkTimeModel>(cancellationToken);
    }

    public async Task<WorkTimeModel?> UpdateAsync(int id, WorkTimeDto dto, CancellationToken cancellationToken = default)
    {
        EnsureAuthorizedRequest();
        var response = await httpClient.PutAsJsonAsync($"api/WorkTime/edit/{id}", dto, cancellationToken);
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<WorkTimeModel>(cancellationToken);
    }

    private static async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var message = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new HttpRequestException(string.IsNullOrWhiteSpace(message)
            ? "Kunne ikke gemme arbejdstid."
            : message.Trim('"'));
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        EnsureAuthorizedRequest();
        var response = await httpClient.DeleteAsync($"api/WorkTime/delete/{id}", cancellationToken);
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