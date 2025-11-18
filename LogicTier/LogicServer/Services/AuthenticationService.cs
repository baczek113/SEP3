using HireFire.Grpc;
using Grpc.Net.Client;
using System.Security.Cryptography;
using System.Text;
using Grpc.Core;
using LogicServer.DTOs.Authentication;
using LogicServer.DTOs.Representative;

namespace LogicServer.Services;

public class AuthenticationService
{
    private readonly string _grpcAddress;
    
    public AuthenticationService(IConfiguration config)
    {
        _grpcAddress = config["GrpcSettings:AuthenticationServiceUrl"] ?? "http://localhost:9090";
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
    {
        try
        {
            using var channel = GrpcChannel.ForAddress(_grpcAddress);
            var client = new HireFire.Grpc.AuthenticationService.AuthenticationServiceClient(channel);

            var passwordHash = HashPassword(loginRequest.Password);

            var request = new LoginRequest
            {
                Email = loginRequest.Email,
                PasswordHash = passwordHash
            };

            var response = await client.LoginAsync(request);

            return new LoginResponseDto()
            {
                Id = unchecked((int)(response.Id)),
                Role = response.Role,
                Name = response.Name,
                Email = response.Email,
                Success = true
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return new LoginResponseDto()
        {
            Success = false
        };
    }
    
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
}