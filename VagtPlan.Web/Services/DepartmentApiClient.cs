using System.Net.Http.Json;
using VagtPlan.Web.Models;

namespace VagtPlan.Web.Services;

public class DepartmentApiClient(HttpClient httpClient)
{
    public async Task<DepartmentModel[]> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var departments = await httpClient.GetFromJsonAsync<DepartmentModel[]>("api/Department/get", cancellationToken);
        return departments ?? [];
    }

    public async Task<DepartmentModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<DepartmentModel>($"api/Department/get/{id}", cancellationToken);
    }

    public async Task<DepartmentModel?> CreateAsync(DepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/Department/createDepartment", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DepartmentModel>(cancellationToken);
    }

    public async Task<DepartmentModel?> UpdateAsync(int id, DepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"api/Department/edit/{id}", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DepartmentModel>(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"api/Department/delete/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
