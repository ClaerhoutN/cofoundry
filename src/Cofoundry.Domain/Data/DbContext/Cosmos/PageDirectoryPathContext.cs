using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos;

namespace Cofoundry.Domain.Data.Cosmos;
public class PageDirectoryPathContext : DbContext {
    /// <summary>
    /// Information about the full directory path and it's position in the directory 
    /// heirachy. This table is automatically updated whenever changes are made to the page 
    /// directory heirarchy and should be treated as read-only.
    /// </summary>
    public DbSet<PageDirectoryPath> PageDirectoryPaths { get; set; }

    private readonly ICofoundryDbContextInitializer _cofoundryDbContextInitializer;
    public PageDirectoryPathContext(ICofoundryDbContextInitializer cofoundryDbContextInitializer) {
        _cofoundryDbContextInitializer = cofoundryDbContextInitializer;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        _cofoundryDbContextInitializer.ConfigureCosmos(this, optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var entity = modelBuilder.Entity<PageDirectoryPath>();
        entity
            .HasNoDiscriminator()
            .ToContainer(nameof(PageDirectoryPath));
        entity.HasKey(x => x.PageDirectoryId);
        entity.Property(x => x.PageDirectoryId).HasConversion(x => x.ToString(), x => int.Parse(x));
        entity.HasPartitionKey(x => x.PageDirectoryId);
    }
}
