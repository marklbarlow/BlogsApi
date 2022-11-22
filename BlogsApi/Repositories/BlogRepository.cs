using BlogsApi.Model;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlogsApi.Repositories;

public interface IBlogRepository
{
    Task AddBlogEntry(string title, string text, int userId);
    Task AddCommentToBlogEntry(int blogId, string text, int userId);
    Task AddLike(int blogId, int userId);
    Task<IEnumerable<BlogEntry>> GetBlogEntries(int top = 5);
    Task<BlogEntry?> GetBlogEntry(int blogId);
    Task<IEnumerable<BlogComment>> GetCommentsForBlogEntry(int blogId);
    Task<IEnumerable<BlogLike>> GetLikesForBlogEntry(int blogId);
    Task<IEnumerable<User>> GetUsers();
    Task RemoveLike(int blogId, int userId);
}

public class BlogRepository : IBlogRepository
{
    private readonly SqlConnectionStringBuilder _builder = new();

    public BlogRepository()
    {
        _builder.DataSource = "bluekid.database.windows.net";
        _builder.UserID = "CloudSA425c211c";
        _builder.Password = "sudRu4-piwgiz-xamgug";
        _builder.InitialCatalog = "blogs";
    }

    public async Task AddBlogEntry(string title, string text, int userId)
    {
        try
        {
            await using var conn = new SqlConnection(_builder.ConnectionString);
            await conn.ExecuteAsync(
                @"INSERT INTO [BlogEntry] (Title, Text, Timestamp, UserId) VALUES (@Title, @Text, @Timestamp, @UserId)",
                new { Title = title, Text = text, Timestamp = DateTime.UtcNow, UserId = userId });
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public async Task AddCommentToBlogEntry(int blogId, string text, int userId)
    {
        try
        {
            await using var conn = new SqlConnection(_builder.ConnectionString);
            await conn.ExecuteAsync(
                @"INSERT INTO [BlogComment] (BlogEntryId, Text, Timestamp, UserId) VALUES (@BlogEntryId, @Text, @Timestamp, @UserId)",
                new { BlogEntryId = blogId, Text = text, Timestamp = DateTime.UtcNow, UserId = userId });
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public async Task AddLike(int blogId, int userId)
    {
        try
        {
            await using var conn = new SqlConnection(_builder.ConnectionString);
            await conn.ExecuteAsync("INSERT INTO [BlogLike] (BlogEntryId, UserId) VALUES (@BlogEntryId, @UserId)",
                new { BlogEntryId = blogId, UserId = userId });
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public async Task<IEnumerable<BlogEntry>> GetBlogEntries(int top = 5)
    {
        try
        {
            await using var conn = new SqlConnection(_builder.ConnectionString);
            return await conn.QueryAsync<BlogEntry>(
                "SELECT TOP (@Top) B.Id, B.Title, B.Text, B.Timestamp, U.Id as UserId, U.Name Username FROM [BlogEntry] B INNER JOIN [User] U on B.UserId = U.Id ORDER BY B.Timestamp DESC",
                new { Top = top });
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }

        return Enumerable.Empty<BlogEntry>();
    }

    public async Task<BlogEntry?> GetBlogEntry(int blogId)
    {
        try
        {
            await using var conn = new SqlConnection(_builder.ConnectionString);
            return await conn.QueryFirstOrDefaultAsync<BlogEntry>(
                "SELECT B.Id, B.Title, B.Text, B.Timestamp, U.Id as UserId, U.Name as Username FROM [BlogEntry] B INNER JOIN [User] U on B.UserId = U.Id WHERE B.Id=@Id",
                new { Id = blogId });
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }

        return default;
    }

    public async Task<IEnumerable<BlogComment>> GetCommentsForBlogEntry(int blogId)
    {
        try
        {
            await using var conn = new SqlConnection(_builder.ConnectionString);
            return await conn.QueryAsync<BlogComment>(
                "SELECT B.Id, B.Text, B.Timestamp, U.Id as UserId, U.Name as Username, B.BlogEntryId FROM [BlogComment] B INNER JOIN [User] U on B.UserId = U.Id WHERE BlogEntryId=@Id ORDER BY Timestamp",
                new { Id = blogId });
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }

        return Enumerable.Empty<BlogComment>();
    }

    public async Task<IEnumerable<BlogLike>> GetLikesForBlogEntry(int blogId)
    {
        try
        {
            await using var conn = new SqlConnection(_builder.ConnectionString);
            return await conn.QueryAsync<BlogLike>(
                "SELECT B.BlogEntryId, U.Id as UserId, U.Name as Username FROM [BlogLike] B INNER JOIN [User] U on B.UserId = U.Id WHERE B.BlogEntryId=@Id", new { Id = blogId });
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }

        return Enumerable.Empty<BlogLike>();
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        try
        {
            await using var conn = new SqlConnection(_builder.ConnectionString);
            return await conn.QueryAsync<User>("SELECT Id, Name FROM [User]");
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }

        return Enumerable.Empty<User>();
    }

    public async Task RemoveLike(int blogId, int userId)
    {
        try
        {
            await using var conn = new SqlConnection(_builder.ConnectionString);
            await conn.ExecuteAsync(@"DELETE FROM [BlogLike] WHERE BlogEntryId = @BlogId AND UserId = @UserId",
                new { BlogId = blogId, UserId = userId });
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}