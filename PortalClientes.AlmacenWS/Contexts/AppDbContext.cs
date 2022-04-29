using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PortalClientes.AlmacenWS.Models;

namespace PortalClientes.AlmacenWS.Contexts {
    public class AppDbContext : IdentityDbContext {

        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("RemoteConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public DbSet<UserData> UserDatas { get; set; }

        public DbSet<UserConfig> UserConfigs { get; set; }

        public DbSet<UserCustomer> UserCustomers { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserData>().HasKey(u => new { u.UserDataID });

            modelBuilder.Entity<UserData>()
                .HasOne<IdentityUser>(u => u.User);

            modelBuilder.Entity<UserConfig>().HasKey(u => new { u.UserConfigID });

            modelBuilder.Entity<UserConfig>()
                .HasOne<IdentityUser>(u => u.User);

            modelBuilder.Entity<UserCustomer>()
                .HasKey(uc => new { uc.UserCustomerID });

            modelBuilder.Entity<UserConfig>()
                .HasOne(uc => uc.RelatedUserConfig)
                .WithMany(uc => uc.RelatedUserConfigs)
                .IsRequired(false);
        }
    }
}
