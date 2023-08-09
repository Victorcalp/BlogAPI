using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogAPI.Data.Mappings
{
    public class PostMap : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Post");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().UseIdentityColumn();

            builder.Property(x => x.LastUpdateDate)
                .IsRequired()
                .HasColumnName("LastUpdateDate")
                .HasColumnType("SMALLDATETIME")
                .HasDefaultValueSql("GETDATE()"); //função do SQL Server
                                                  // .HasDefaultValue(DateTime.Now.ToUniversalTime()); //função do .NET

            //Indice
            builder.HasIndex(x => x.Slug, "IX_Post_Slug").IsUnique();

            //Relacionamento UM para Muitos
            builder.HasOne(x => x.Author)
                .WithMany(x => x.Posts)
                .HasConstraintName("FK_Post_Author")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Posts)
                .HasConstraintName("FK_Post_Category")
                .OnDelete(DeleteBehavior.Cascade);

            //Relacionamento N para N (muito para muitos)
            builder.HasMany(x => x.Tags)
                .WithMany(x=>x.Posts)
                //cria uma entidade virtual
                //Gera uma tabela chamada "PostTag" com dois campos "PostId" e "TagId" com duas foreignKey
                .UsingEntity<Dictionary<string, object>>("PostTag", 
                post => post.HasOne<Tag>() //tag tem 1 post
                    .WithMany() //tag tem muitas posts
                    .HasForeignKey("PostId")
                    .HasConstraintName("FK_PostTag_PostId")
                    .OnDelete(DeleteBehavior.Cascade),
                tag => tag.HasOne<Post>() //Tag tem 1 post
                    .WithMany() // Post tem muitas Tags
                    .HasForeignKey("TagId")
                    .HasConstraintName("FK_PostTag_TagId")
                    .OnDelete(DeleteBehavior.Cascade)
                );
        }
    }
}