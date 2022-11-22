namespace BlogsApi.Model;

public class BlogComment
{
    public int BlogEntryId { get; set; }

    public int Id { get; set; }

    public string Text { get; set; } = "";

    public DateTime Timestamp { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = "";
}