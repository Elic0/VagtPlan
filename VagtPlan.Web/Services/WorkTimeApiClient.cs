using System.Net.Http.Json;
using VagtPlan.Web.Models;

namespace VagtPlan.Web.Services;

public class WorkTimeApiClient(HttpClient httpClient)
{
    public async Task<WorkTimeModel[]> GetByDepartmentAsync(long departmentId, CancellationToken cancellationToken = default)
    {
        var workTimes = await httpClient.GetFromJsonAsync<WorkTimeModel[]>(
            $"api/WorkTime/get/byDepartment/{departmentId}",
            cancellationToken);

        return workTimes ?? [];
    }

    public async Task<WorkTimeModel?> CreateAsync(WorkTimeDto dto, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/WorkTime/createWorkTime", dto, cancellationToken);
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<WorkTimeModel>(cancellationToken);
    }

    public async Task<WorkTimeModel?> UpdateAsync(long id, WorkTimeDto dto, CancellationToken cancellationToken = default)
    {
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

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"api/WorkTime/delete/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}