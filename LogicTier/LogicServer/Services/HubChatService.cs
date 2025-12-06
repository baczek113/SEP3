using System.Security.Claims;
using Grpc.Core;
using HireFire.Grpc;
using LogicServer.DTOs.Chat;
using Microsoft.AspNetCore.SignalR;
namespace LogicServer.Services;

public class HubChatService : Hub
{
    private readonly ChatService.ChatServiceClient _grpcClient;
    
    public HubChatService(ChatService.ChatServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task JoinChat(long jobId, long applicantId)
    {
        
        try
        {          
            var handshakeRequest = new ChatHandshakeRequest { JobId = jobId, ApplicantId = applicantId };
            var handshakeResponse = await _grpcClient.GetChatHandshakeAsync(handshakeRequest);
            if (!handshakeResponse.Exists)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "No application found.");
                return;
            }
            
            long applicationId = handshakeResponse.ApplicationId;

            string groupName = $"app_{applicationId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            
            var request = new GetMessagesRequest{ApplicationId = applicationId};
            var response = await _grpcClient.GetMessagesAsync(request);
            
            var history = response?.Messages.Select(m => new ChatMessageDTO
            {
                SenderName = m.SenderName,
                Body = m.Body,
                SendAt = m.SendAt, 
                SenderId = m.SenderId
            }).ToList() ?? new List<ChatMessageDTO>();
            await Clients.Caller.SendAsync("ReceiveHistory", history, applicationId);

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