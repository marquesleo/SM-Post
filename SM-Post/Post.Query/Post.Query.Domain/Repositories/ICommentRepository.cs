using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories;

public interface ICommentRepository
{
    Task CreateComment(CommentEntity comment);
    
    Task UpdateComment(CommentEntity comment);
    
    Task<CommentEntity> GetComment(Guid commentId);
    
    Task DeleteComment(Guid commentId);
    
}