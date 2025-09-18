using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;

namespace Post.Query.Infrastructure.DataAccess;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            //"ConnectionString": "server=localhost;uid=root;pwd=13023722;database=vestillo",
            //Server=localhost,1433;Database=SocialMedia;User Id=sa;Password=Leo141827;

            optionsBuilder.UseMySQL("server=localhost;uid=root;pwd=13023722;database=SocialMedia")
                .UseLazyLoadingProxies();
        }
    }

    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }
    
}