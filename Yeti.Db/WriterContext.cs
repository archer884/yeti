using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using Yeti.Db.Model;

namespace Yeti.Db;

public class WriterContext(DbContextOptions<WriterContext> options) : DbContext(options)
{
    public DbSet<Writer> Writers { get; set; } = null!;
    public DbSet<Login> Logins { get; set; } = null!;
    public DbSet<Manuscript> Manuscripts { get; set; } = null!;
    public DbSet<Fragment> Fragments { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;

    // FIXME: override Remove to provide soft delete?

    public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
    {
        if (entity is Tracked tracked)
        {
            tracked.Updated = DateTimeOffset.UtcNow;
        }

        return base.Update(entity);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Writer>(e =>
        {
            e.HasOne(x => x.Login).WithOne(x => x.Writer);
            e.HasMany(x => x.Manuscripts).WithOne(x => x.Writer);
        });

        builder.Entity<Login>().HasOne(x => x.Writer).WithOne(x => x.Login).HasForeignKey<Writer>(x => x.LoginId);

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
            LoginId = 1,
            Username = "longfellow",
            AuthorName = "H. Wadsworth Longfellow",
        });

        builder.Entity<Login>().HasData(new Login
        {
            Id = 1,
            WriterId = 1,
            Serialized = "$argon2id$v=19$m=19456,t=2,p=1$qbl/yTz6vxqviqM2SB9/wQ$qxbxQscW0sy907L8PYeNsTNTxyMZQuLm2r2ZONNFXWk",
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
