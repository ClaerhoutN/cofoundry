using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos;

namespace Cofoundry.Domain.Data.Cosmos;
public class LocaleContext : DbContext {
    public DbSet<Locale> Locales { get; set; }

    private readonly ICofoundryDbContextInitializer _cofoundryDbContextInitializer;
    public LocaleContext(ICofoundryDbContextInitializer cofoundryDbContextInitializer) {
        _cofoundryDbContextInitializer = cofoundryDbContextInitializer;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        _cofoundryDbContextInitializer.ConfigureCosmos(this, optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var entity = modelBuilder.Entity<Locale>();
        entity
            .HasNoDiscriminator()
            .ToContainer(nameof(Locale));
        entity.HasKey(x => x.LocaleId);
        entity.Property(x => x.LocaleId).HasConversion(x => x.ToString(), x => int.Parse(x));
        entity.HasPartitionKey(x => x.LocaleId);
    }
}
