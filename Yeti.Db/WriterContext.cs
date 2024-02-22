using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using Yeti.Db.Model;

namespace Yeti.Db;

public class WriterContext(DbContextOptions<WriterContext> options)
    : DbContext(options)
{
    public DbSet<Writer> Writers { get; set; } = null!;
    public DbSet<Manuscript> Manuscripts { get; set; } = null!;
    public DbSet<Fragment> Fragments { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;

    public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
    {
        if (entity is Tracked tracked)
        {
            tracked.Updated = DateTimeOffset.Now;
        }

        return base.Update(entity);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Writer>().HasMany(x => x.Manuscripts).WithOne(x => x.Writer);

        builder.Entity<Manuscript>(e =>
        {
            e.HasMany(x => x.Fragments).WithOne(x => x.Manuscript);
            e.HasIndex(x => x.Title);
            e.HasIndex(x => x.Created);
            e.HasIndex(x => x.SoftDelete);
        });

        builder.Entity<Fragment>(e =>
        {
            e.HasOne(x => x.Manuscript).WithMany(x => x.Fragments);
            e.HasIndex(x => x.Created);
            e.HasIndex(x => x.SoftDelete);
        });

        builder.Entity<Tag>().HasIndex(x => x.Value).IsUnique();

        Seed(builder);

        base.OnModelCreating(builder);
    }

    private static void Seed(ModelBuilder builder)
    {
        builder.Entity<Writer>().HasData(new Writer
        {
            Id = 1,
            Username = "longfellow",
            AuthorName = "H. Wadsworth Longfellow",
        });

        builder.Entity<Manuscript>().HasData(new Manuscript
        {
            Id = 1,
            WriterId = 1,
            Title = "The Song of Hiawatha",
            Blurb = "An old poem",
        });

        var deletedFragment = new Fragment
        {
            Id = 1,
            WriterId = 1,
            ManuscriptId = 1,
            Content = "By the shore of Gitche Gumbee,\nBy the shining Big-Sea-Water,\nAt the doorway of his wigwam,\nIn the pleasant Summer morning,\nHiawatha stood and waited.",
            SortBy = 1.0,
            SoftDelete = true,
        };

        var fragment = new Fragment
        {
            Id = 2,
            WriterId = 1,
            ManuscriptId = 1,
            Content = "By the shore of Gitche Gumee,\nBy the shining Big-Sea-Water,\nAt the doorway of his wigwam,\nIn the pleasant Summer morning,\nHiawatha stood and waited.",
            SortBy = 1.0,
        };

        builder.Entity<Fragment>().HasData(deletedFragment, fragment);
    }
}
