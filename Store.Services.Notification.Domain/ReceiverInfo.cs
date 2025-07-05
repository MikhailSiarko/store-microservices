namespace Store.Services.Notification.Domain;

public class ReceiverInfo
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}