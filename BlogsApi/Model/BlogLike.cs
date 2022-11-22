namespace BlogsApi.Model;

public class BlogLike
{
    public int BlogEntryId { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = "";
}