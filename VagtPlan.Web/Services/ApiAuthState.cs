using Microsoft.JSInterop;

namespace VagtPlan.Web.Services;

public class ApiAuthState
{
    private IJSRuntime? _js;
    private bool _isInitialized;
    private readonly TaskCompletionSource _initTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public string? Token { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);

    public bool IsInitialized => _isInitialized;

    public Task WhenReadyAsync() => _initTcs.Task;

    public event Action? Changed;

    public async Task InitializeAsync(IJSRuntime js)
    {
        if (_isInitialized)
        {
            return;
        }

        _js = js;

        try
        {
            var token = await js.InvokeAsync<string?>("authStorage.getToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                Token = token;
            }
        }
        catch (JSException)
        {
            // JS not available yet or storage blocked
        }

        _isInitialized = true;
        _initTcs.TrySetResult();
        Changed?.Invoke();
    }

    public void SetToken(string token)
    {
        Token = token;
        _ = PersistTokenAsync(token);
        Changed?.Invoke();
    }

    public void Clear()
    {
        Token = null;
        _ = RemoveTokenAsync();
        Changed?.Invoke();
    }

    private async Task PersistTokenAsync(string token)
    {
        if (_js is null)
        {
            return;
        }

        try
        {
            await _js.InvokeVoidAsync("authStorage.setToken", token);
        }
        catch (JSException)
        {
            // ignore persistence errors
        }
    }

    private async Task RemoveTokenAsync()
    {
        if (_js is null)
        {
            return;
        }

        try
        {
            await _js.InvokeVoidAsync("authStorage.removeToken");
        }
        catch (JSException)
        {
            // ignore persistence errors
        }
    }
}
