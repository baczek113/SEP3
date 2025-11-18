using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Services;
using WebApp.DTOs.Authentication;

namespace WebApp.Authentication
{
    public class AuthProvider : AuthenticationStateProvider
    {
        private readonly HttpClient client;
        private ClaimsPrincipal currentClaimsPrincipal = new(new ClaimsIdentity());

        public AuthProvider(HttpClient client)
        {
            this.client = client;
        }

        public async Task LoginAsync(LoginRequestDto request)
        {
            HttpResponseMessage httpResponse = await client.PostAsJsonAsync("api/authentication", request);
            string response = await httpResponse.Content.ReadAsStringAsync();
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception(response);
            }

            LoginResponseDto loginResponseDto = JsonSerializer.Deserialize<LoginResponseDto>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            if (!loginResponseDto.Success)
            {
                throw new Exception("Incorrect credentials");
            }

            List<Claim> claims = new List<Claim>()
            {
                new(ClaimTypes.Name, loginResponseDto.Name),
                new("Role",  loginResponseDto.Role),
                new("Email", request.Email)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth");

            currentClaimsPrincipal = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(currentClaimsPrincipal)));
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(currentClaimsPrincipal));
        }

        public void Logout()
        {
            currentClaimsPrincipal = new(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(currentClaimsPrincipal)));
        }
    }
}
