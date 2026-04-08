using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;

public class TokenService
{
    private readonly IJSRuntime _js;

    public TokenService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task SaveTokenAsync(string token)
    {
        await _js.InvokeVoidAsync("auth.saveToken", token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _js.InvokeAsync<string>("auth.getToken");
    }

    public async Task RemoveTokenAsync()
    {
        await _js.InvokeVoidAsync("auth.removeToken");
    }

    public (string userName, string email, string role) DecodeToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return ("", "", "");

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        return (
            jwt.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value ?? "",
            jwt.Claims.FirstOrDefault(c => c.Type == "Email")?.Value ?? "",
            jwt.Claims.FirstOrDefault(c => c.Type == "Role")?.Value ?? ""
        );
    }
}
