namespace Store.Services.Notification.Data;

public class Notification
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public required string Email { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public bool IsSent { get; set; }
}