namespace BlogsApi.Model;

public class BlogEntry
{
    public int Id { get; set; }

    public string Text { get; set; } = "";

    public DateTime Timestamp { get; set; }

    public string Title { get; set; } = "";

    public int UserId { get; set; }

    public string Username { get; set; } = "";
}