using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos;

namespace Cofoundry.Domain.Data.Cosmos;
public class PageDirectoryAccessRuleContext : DbContext {
    /// <summary>
    /// <para>
    /// Access rules are used to restrict access to a website resource to users
    /// fulfilling certain criteria such as a specific user area or role. Page
    /// directory access rules are used to define the rules at a <see cref="PageDirectory"/> 
    /// level. These rules are inherited by child directories and pages.
    /// </para>
    /// <para>
    /// Note that access rules do not apply to users from the Cofoundry Admin user
    /// area. They aren't intended to be used to restrict editor access in the admin UI 
    /// but instead are used to restrict public access to website pages and routes.
    /// </para>
    /// </summary>
    public DbSet<PageDirectoryAccessRule> PageDirectoryAccessRules { get; set; }

    private readonly ICofoundryDbContextInitializer _cofoundryDbContextInitializer;
    public PageDirectoryAccessRuleContext(ICofoundryDbContextInitializer cofoundryDbContextInitializer) {
        _cofoundryDbContextInitializer = cofoundryDbContextInitializer;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        _cofoundryDbContextInitializer.ConfigureCosmos(this, optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var entity = modelBuilder.Entity<PageDirectoryAccessRule>();
        entity
            .HasNoDiscriminator()
            .ToContainer(nameof(PageDirectoryAccessRule));
        entity.HasKey(x => x.PageDirectoryAccessRuleId);
        entity.Property(x => x.PageDirectoryAccessRuleId).HasConversion(x => x.ToString(), x => int.Parse(x));
        entity.HasPartitionKey(x => x.PageDirectoryAccessRuleId);
    }
}
