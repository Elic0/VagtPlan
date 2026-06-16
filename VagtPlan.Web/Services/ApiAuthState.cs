namespace VagtPlan.Web.Services;

public class ApiAuthState
{
    public string? Token { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);

    public event Action? Changed;

    public void SetToken(string token)
    {
        Token = token;
        Changed?.Invoke();
    }

    public void Clear()
    {
        Token = null;
        Changed?.Invoke();
    }
}
