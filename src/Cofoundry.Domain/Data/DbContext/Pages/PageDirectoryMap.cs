using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class PageDirectoryMap : IEntityTypeConfiguration<PageDirectory>
{
    public void Configure(EntityTypeBuilder<PageDirectory> builder)
    {
        builder.ToTable(nameof(PageDirectory), DbConstants.CofoundrySchema);
        builder.HasKey(s => s.PageDirectoryId);

        builder.Property(s => s.Name)
            .HasMaxLength(200);

        builder.Property(s => s.UrlPath)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(s => s.ParentPageDirectory)
            .WithMany(d => d.ChildPageDirectories)
            .HasForeignKey(s => s.ParentPageDirectoryId);

        CreateAuditableMappingHelper.Map(builder);
    }
}
