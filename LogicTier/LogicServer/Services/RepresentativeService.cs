using HireFire.Grpc;
using Grpc.Net.Client;
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
        Console.WriteLine("LOGIC: Mapping RepresentativeDto to gRPC request for DatabaseServer");
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.RepresentativeService.RepresentativeServiceClient(channel);

        var passwordHash = AuthenticationService.HashPassword(dto.Password);
        
        var request = new CreateRepRequest
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = passwordHash,
            Position = dto.Position
        };
        Console.WriteLine("LOGIC: Sending CreateRepresentative gRPC request to DatabaseServer");
        var reply = await client.CreateRepresentativeAsync(request);
        Console.WriteLine("LOGIC: Received CreateRepresentative gRPC response from DatabaseServer");
        return new RepresentativeDto()
        {
            Id = reply.Id,
            Name = reply.Name,
            Email = reply.Email,
            Position = reply.Position
        };
        
    }

    public async Task<RepresentativeDto> UpdateRepresentativeAsync(UpdateRepresentativeDto dto)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.RepresentativeService.RepresentativeServiceClient(channel);

        var passwordHash = string.IsNullOrEmpty(dto.Password) 
            ? "" 
            : AuthenticationService.HashPassword(dto.Password);

        var request = new UpdateRepRequest
        {
            Id = dto.Id,
            Name = dto.Name,
            Email = dto.Email,
            Position = dto.Position,
            PasswordHash = passwordHash
        };

        var reply = await client.UpdateRepresentativeAsync(request);

        return new RepresentativeDto()
        {
            Id = reply.Id,
            Name = reply.Name,
            Email = reply.Email,
            Position = reply.Position
        };
    }

    public async Task<bool> DeleteRepresentativeAsync(long id)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.RepresentativeService.RepresentativeServiceClient(channel);

        var request = new RemoveRepresentativeRequest
        {
            Id = id
        };

        var reply = await client.DeleteRepresentativeAsync(request);

        return reply.Success;
    }
}
