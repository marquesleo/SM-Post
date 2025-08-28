using MongoDB.Driver.Linq;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories;

public class CommentRepository  :ICommentRepository
{
    private readonly DatabaseContextFactory _contextFactory;

    public CommentRepository(DatabaseContextFactory contextFactory)
    {
        this._contextFactory = contextFactory;
    }
    public async Task CreateComment(CommentEntity comment)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Comments.Add(comment);
        await context.SaveChangesAsync();
    }

    public async Task UpdateComment(CommentEntity comment)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Comments.Update(comment);
        _=  await context.SaveChangesAsync();
    }

    public async  Task<CommentEntity> GetComment(Guid commentId)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();

        return await context.Comments.FirstOrDefaultAsync(p => p.CommentId == commentId);

    }

    public async Task DeleteComment(Guid commentId)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        var comment = await GetComment(commentId);
        if (comment == null) return;
        context.Comments.Remove(comment);
        _=  await context.SaveChangesAsync();
    }
}