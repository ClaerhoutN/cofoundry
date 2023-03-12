using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos;

namespace Cofoundry.Domain.Data.Cosmos;
public class UserAreaContext : DbContext {
    /// <summary>
    /// Users can be partitioned into different 'User Areas' that enabled the identity system use by the Cofoundry administration area 
    /// to be reused for other purposes, but this isn't a common scenario and often there will only be the Cofoundry UserArea. UserAreas
    /// are defined in code by defining an IUserAreaDefinition
    /// </summary>
    public DbSet<UserArea> UserAreas { get; set; }

    private readonly ICofoundryDbContextInitializer _cofoundryDbContextInitializer;
    public UserAreaContext(ICofoundryDbContextInitializer cofoundryDbContextInitializer) 
    {
        _cofoundryDbContextInitializer = cofoundryDbContextInitializer;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        _cofoundryDbContextInitializer.ConfigureCosmos(this, optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var entity = modelBuilder.Entity<UserArea>();
        entity
            .HasNoDiscriminator()
            .ToContainer(nameof(UserArea));
        entity.HasKey(x => x.UserAreaCode);
        entity.HasPartitionKey(x => x.UserAreaCode);
    }
}
