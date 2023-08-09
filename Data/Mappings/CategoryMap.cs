using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogAPI.Data.Mappings
{
    public class CategoryMap : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            //tabela
            builder.ToTable("Category");

            //Informa o ID como chave primaria 
            builder.HasKey(c => c.Id);

            //identity(1, 1)
            builder.Property(c => c.Id).ValueGeneratedOnAdd().UseIdentityColumn();

            //Propriedades
            builder.Property(c => c.Name)
                .IsRequired() //not null
                .HasColumnName("Name")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(80);

            builder.Property(c => c.Slug)
                .IsRequired()
                .HasColumnName("Slug")
                .HasColumnType("VARCHAR")
                .HasMaxLength(80);

            //Indice
            builder.HasIndex(x => x.Slug, "IX_Category_Slug").IsUnique();
        }
    }
}