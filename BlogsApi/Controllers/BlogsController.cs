using BlogsApi.Model;
using BlogsApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BlogsApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BlogsController : ControllerBase
{
    private readonly ILogger<BlogsController> _logger;
    private readonly IBlogRepository _repository;

    public BlogsController(ILogger<BlogsController> logger, IBlogRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpPost("blogs")]
    public async Task AddBlogEntry([FromBody] BlogSubmission submission)
    {
        await _repository.AddBlogEntry(submission.Title, submission.Text, submission.UserId);
    }

    [HttpPost("blogs/{blogId}/comments")]
    public async Task AddComment(int blogId, [FromBody] BlogCommentSubmission submission)
    {
        await _repository.AddCommentToBlogEntry(blogId, submission.Text, submission.UserId);
    }

    [HttpPut("blogs/{blogId}/likes/{userId}")]
    public async Task AddLike(int blogId, int userId)
    {
        await _repository.AddLike(blogId, userId);
    }

    [HttpGet("blogs")]
    public async Task<IEnumerable<BlogSummary>> GetBlogSummaries(int top = 5)
    {
        var entries = await _repository.GetBlogEntries(top);
        return entries.Select(Extensions.ToSummary);
    }

    [HttpGet("blogs/{id}")]
    public async Task<BlogEntry?> GetBlogEntry(int id)
    {
        return await _repository.GetBlogEntry(id);
    }

    [HttpGet("blogs/{blogId}/comments")]
    public async Task<IEnumerable<BlogComment>> GetComments(int blogId)
    {
        return await _repository.GetCommentsForBlogEntry(blogId);
    }

    [HttpGet("blogs/{blogId}/likes")]
    public async Task<IEnumerable<BlogLike>> GetLikes(int blogId)
    {
        return await _repository.GetLikesForBlogEntry(blogId);
    }

    [HttpGet("users")]
    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _repository.GetUsers();
    }

    [HttpDelete("blogs/{blogId}/likes/{userId}")]
    public async Task RemoveLike(int blogId, int userId)
    {
        await _repository.RemoveLike(blogId, userId);
    }
}