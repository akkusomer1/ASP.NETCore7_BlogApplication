using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgrammersBlog.Core.Concrete.Entities;

namespace ProgrammersBlog.Data.Configurations
{
    public class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {   
            builder.HasKey(x=> x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x=>x.Title).IsRequired().HasMaxLength(100);
            builder.Property(x=>x.Content).IsRequired().HasColumnType("NVARCHAR(MAX)");

            builder.Property(X => X.Date).IsRequired();
            builder.Property(X => X.SeoAuthor).IsRequired().HasMaxLength(50);
            builder.Property(X => X.SeoDescription).IsRequired().HasMaxLength(150);
            builder.Property(X => X.SeoTags).IsRequired().HasMaxLength(70);
            builder.Property(X => X.ViewsCount).IsRequired();
            builder.Property(X => X.CommentCount).IsRequired();
            builder.Property(X => X.Thumbnail).IsRequired().HasMaxLength(250);

            builder.Property(x => x.CreatedByName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.UpdateByName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.UpdateDate).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.Property(x => x.Note).HasMaxLength(500);


            builder.HasOne(x=>x.Category).WithMany(x=>x.Articles).HasForeignKey(x=>x.CategoryId);
            builder.HasOne(x=>x.User).WithMany(x=>x.Articles).HasForeignKey(x=>x.UserId);

            builder.ToTable("Articles");

        

        }
    }
}
