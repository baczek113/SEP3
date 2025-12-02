namespace WebApp.DTOs.Chat;

public class ChatMessageDto
{
    public string SenderName { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string SendAt { get; set; } = string.Empty;
    public long SenderId { get; set; }
}