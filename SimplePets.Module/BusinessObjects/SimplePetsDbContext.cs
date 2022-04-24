using System;
using DevExpress.ExpressApp.EFCore.Updating;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.EFCore.DesignTime;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.Persistent.Base;

namespace SimplePets.Module.BusinessObjects {


    public abstract class Pet
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsCat { get; set; }
    }


    public class Kitten : BabyPet
    {
        
        public virtual Cat Parent { get; set; }  // not sure if I should have new here
    }
    [NavigationItem("Pets")]
    public class Dog : Pet
    {
       
        public Dog()
        {
            Puppies = new List<Puppy>();
        }
        [Aggregated]
        public virtual List<Puppy> Puppies { get; set; }
    }

    [NavigationItem("Pets")]
    public class Cat : Pet
    {
        public Cat()
        {
            Kittens = new List<Kitten>();
        }

       
        [Aggregated]
        public virtual List<Kitten> Kittens { get; set; }
    }

    public abstract class BabyPet
    {


        [Key] public int Id { get; set; }

        public int ParentPetId { get; set; }

        [ForeignKey("ParentPetId")]
        public virtual Pet Parent { get; set; }
        public string Name { get; set; }
        public bool? IsCat { get; set; }

    }
    public class Puppy : BabyPet
    {
        public virtual Dog Parent { get; set; }
    }

    // This code allows our Model Editor to get relevant EF Core metadata at design time.
    // For details, please refer to https://supportcenter.devexpress.com/ticket/details/t933891.
    public class SimplePetsContextInitializer : DbContextTypesInfoInitializerBase {
		protected override DbContext CreateDbContext() {
			var optionsBuilder = new DbContextOptionsBuilder<SimplePetsEFCoreDbContext>()
                .UseSqlServer(@";");
            return new SimplePetsEFCoreDbContext(optionsBuilder.Options);
		}
	}
	//This factory creates DbContext for design-time services. For example, it is required for database migration.
	public class SimplePetsDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SimplePetsEFCoreDbContext> {
		public SimplePetsEFCoreDbContext CreateDbContext(string[] args) {
			throw new InvalidOperationException("Make sure that the database connection string and connection provider are correct. After that, uncomment the code below and remove this exception.");
			//var optionsBuilder = new DbContextOptionsBuilder<SimplePetsEFCoreDbContext>();
			//optionsBuilder.UseSqlServer(@"Integrated Security=SSPI;Pooling=false;Data Source=(localdb)\\mssqllocaldb;Initial Catalog=SimplePets");
			//return new SimplePetsEFCoreDbContext(optionsBuilder.Options);
		}
	}
	[TypesInfoInitializer(typeof(SimplePetsContextInitializer))]
	public class SimplePetsEFCoreDbContext : DbContext {
		public SimplePetsEFCoreDbContext(DbContextOptions<SimplePetsEFCoreDbContext> options) : base(options) {
		}
        public DbSet<Cat> Cats { get; set; }
        public DbSet<Dog> Dogs { get; set; }
        public DbSet<Kitten> Kittens { get; set; }
        public DbSet<Puppy> Puppys { get; set; }
        public DbSet<ModuleInfo> ModulesInfo { get; set; }
		public DbSet<ModelDifference> ModelDifferences { get; set; }
		public DbSet<ModelDifferenceAspect> ModelDifferenceAspects { get; set; }
	    public DbSet<PermissionPolicyRole> Roles { get; set; }
	    public DbSet<SimplePets.Module.BusinessObjects.ApplicationUser> Users { get; set; }
        public DbSet<SimplePets.Module.BusinessObjects.ApplicationUserLoginInfo> UserLoginInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<SimplePets.Module.BusinessObjects.ApplicationUserLoginInfo>(b => {
                b.HasIndex(nameof(DevExpress.ExpressApp.Security.ISecurityUserLoginInfo.LoginProviderName), nameof(DevExpress.ExpressApp.Security.ISecurityUserLoginInfo.ProviderUserKey)).IsUnique();
            });
            modelBuilder.Entity<Pet>()
                  .HasDiscriminator(x => x.IsCat)
                  .HasValue<Cat>(true)
                  .HasValue<Dog>(false);

            modelBuilder.Entity<BabyPet>()
              .HasDiscriminator(x => x.IsCat)
              .HasValue<Kitten>(true)
              .HasValue<Puppy>(false);
            modelBuilder.Entity<Puppy>().HasOne(x => x.Parent).WithMany(x => x.Puppies).HasForeignKey(x => x.ParentPetId);
            modelBuilder.Entity<Kitten>().HasOne(x => x.Parent).WithMany(x => x.Kittens).HasForeignKey(x => x.ParentPetId);
        }
	}
}
