using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos;

namespace Cofoundry.Domain.Data.Cosmos;
public class PageDirectoryLocaleContext : DbContext {
    public DbSet<PageDirectoryLocale> PageDirectoryLocales { get; set; }

    private readonly ICofoundryDbContextInitializer _cofoundryDbContextInitializer;
    public PageDirectoryLocaleContext(ICofoundryDbContextInitializer cofoundryDbContextInitializer) {
        _cofoundryDbContextInitializer = cofoundryDbContextInitializer;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        _cofoundryDbContextInitializer.ConfigureCosmos(this, optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var entity = modelBuilder.Entity<PageDirectoryLocale>();
        entity
            .HasNoDiscriminator()
            .ToContainer(nameof(PageDirectoryLocale));
        entity.HasKey(x => x.PageDirectoryLocaleId);
        entity.Property(x => x.PageDirectoryLocaleId).HasConversion(x => x.ToString(), x => int.Parse(x));
        entity.HasPartitionKey(x => x.PageDirectoryLocaleId);
    }
}