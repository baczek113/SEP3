using Grpc.Core;
using HireFire.Grpc;
using Microsoft.AspNetCore.SignalR;
namespace LogicServer.Services;

public class HubChatService : Hub
{
    private readonly ChatService.ChatServiceClient _grpcClient;
    
    public HubChatService(ChatService.ChatServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task JoinChat(long applicationId)
    {
        try
        {
            string groupName = $"app_{applicationId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            
            var request = new GetMessagesRequest{ApplicationId = applicationId};
            var response = await _grpcClient.GetMessagesAsync(request);
            
            await Clients.Caller.SendAsync("ReceiveHistory", response);

        }
        catch (Exception e)
        {
            Console.WriteLine($"Error joining chat: {e.Message}");
            await Clients.Caller.SendAsync("ErrorMessage", "Could not load chat history.");
        }
    }

    public async Task SendMessage(long applicationId, string message, long senderId)
    {
        try
        {
            var request = new SaveMessagesRequest
            {
                ApplicationId = applicationId,
                SenderId = senderId,
                Body = message
            };
            var response = await _grpcClient.SaveAsync(request);

            if (response.Success)
            {
                string groupName = $"app_{applicationId}";
                await Clients.Group(groupName).SendAsync("ReceiveMessage", response.CreatedMessage.SenderName,
                    response.CreatedMessage.Body, response.CreatedMessage.SendAt, response.CreatedMessage.SenderId);
            }
        }
        catch (RpcException)
        {
            await Clients.Caller.SendAsync("ErrorMessage", "Chat is not active (No match found).");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error sending message: {e.Message}");
        }
    }
}