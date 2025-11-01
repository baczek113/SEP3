using HireFire.Grpc;
using Grpc.Net.Client;
using System.Security.Cryptography;
using System.Text;
using LogicServer.DTOs.Representative;

namespace LogicServer.Services;

public class RepresentativeService
{
    private readonly string _grpcAddress;
    
    public RepresentativeService(IConfiguration config)
    {
        _grpcAddress = config["GrpcSettings:RepresentativeServiceUrl"] ?? "http://localhost:9090";
    }
    public async Task<RepresentativeDto> CreateRepresentativeAsync(CreateRepresentativeDto dto)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.RepresentativeService.RepresentativeServiceClient(channel);

        var passwordHash = HashPassword(dto.Password);

        var request = new CreateRepRequest
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = passwordHash,
            Position = dto.Position
        };

        var reply = await client.CreateRepresentativeAsync(request);

        return new RepresentativeDto()
        {
            Id = reply.Id,
            Name = reply.Name,
            Email = reply.Email,
            Position = reply.Position
        };
    }
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
}