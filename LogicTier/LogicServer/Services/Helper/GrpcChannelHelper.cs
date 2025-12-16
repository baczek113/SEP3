namespace LogicServer.Services.Helper;
using Grpc.Net.Client;
using System.Net.Http;

public static class GrpcChannelHelper
{
    public static HttpClientHandler GetSecureHandler()
    {
        var httpHandler = new HttpClientHandler();
        
        httpHandler.ServerCertificateCustomValidationCallback = 
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        return httpHandler;
    }
    public static GrpcChannel CreateSecureChannel(string address)
    {
        return GrpcChannel.ForAddress(address, new GrpcChannelOptions
        {
            HttpHandler = GetSecureHandler()
        });
    }
}