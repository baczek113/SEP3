namespace LogicServer.DTOs.Chat;

public class ChatMessageDTO
{
    public string SenderName { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string SendAt { get; set; } = string.Empty;
    public long SenderId { get; set; }
}