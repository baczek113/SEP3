using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Services;
using WebApp.DTOs.Authentication;

namespace WebApp.Authentication
{
    public class AuthProvider : AuthenticationStateProvider
    {
        private readonly HttpClient client;
        private readonly IJSRuntime jsRuntime;

        public AuthProvider(HttpClient client, IJSRuntime jsRuntime)
        {
            this.client = client;
            this.jsRuntime = jsRuntime;
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

            string serialisedData = JsonSerializer.Serialize(loginResponseDto); 
            await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", serialisedData);
            
            List<Claim> claims = new List<Claim>()
            {
                new(ClaimTypes.Name, loginResponseDto.Name),
                new("Role",  loginResponseDto.Role),
                new("Email", loginResponseDto.Email),
                new(ClaimTypes.NameIdentifier, loginResponseDto.Id.ToString())
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity); 
            NotifyAuthenticationStateChanged( Task.FromResult(new AuthenticationState(claimsPrincipal)) );
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string userAsJson = "";
            try
            {
                userAsJson = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "currentUser");
            }
            catch (InvalidOperationException e)
            {
                return new AuthenticationState(new());
            }

            if (string.IsNullOrEmpty(userAsJson))
            {
                return new AuthenticationState(new());
            } 
            LoginResponseDto loginResponseDto = JsonSerializer.Deserialize<LoginResponseDto>(userAsJson)!;
            List<Claim> claims = new List<Claim>()
            {
                new(ClaimTypes.Name, loginResponseDto.Name),
                new("Role",  loginResponseDto.Role),
                new("Email", loginResponseDto.Email),
                new(ClaimTypes.NameIdentifier, loginResponseDto.Id.ToString())
            }; 
            ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth"); 
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity); 
            return new AuthenticationState(claimsPrincipal);
        }

        public async void LogoutAsync()
        {
            await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", ""); 
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new())));
        }
    }
}
