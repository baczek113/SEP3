using HireFire.Grpc;
using Grpc.Net.Client;
using System.Security.Cryptography;
using System.Text;
using Grpc.Core;
using LogicServer.DTOs.Authentication;
using LogicServer.DTOs.Representative;
using LogicServer.Services.Helper;
namespace LogicServer.Services;

public class AuthenticationService
{
    private readonly string _grpcAddress;
    
    public AuthenticationService(IConfiguration config)
    {
        _grpcAddress = config["GrpcSettings:AuthenticationServiceUrl"] ?? "https://localhost:9090";
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
    {
        try
        {
            using var channel = GrpcChannelHelper.CreateSecureChannel(_grpcAddress);

            var client = new HireFire.Grpc.AuthenticationService.AuthenticationServiceClient(channel);


            var request = new LoginRequest
            {
                Email = loginRequest.Email,
                PasswordHash = loginRequest.Password
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
}