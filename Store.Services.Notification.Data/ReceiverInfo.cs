namespace Store.Services.Notification.Data;

public class ReceiverInfo
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}