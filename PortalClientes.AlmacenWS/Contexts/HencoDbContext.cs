using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PortalClientes.AlmacenWS.Models;

namespace PortalClientes.AlmacenWS.Contexts {
    public class HencoDbContext : DbContext {
        public HencoDbContext() { }

        public HencoDbContext(DbContextOptions<HencoDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("HencoConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<CustomerGroupCustomer> CustomerGroupCustomers { get; set; }

        public DbSet<CustomerGroup> CustomerGroups { get; set; }

        public DbSet<Division> Divisions { get; set; }

        public DbSet<Branch> Branches { get; set; }

        public DbSet<ApiKey> ApiKeys { get; set; }

        public DbSet<WebApiStatistic> WebApiStatistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>(entity => {
                entity.HasKey(c => new { c.CompanyID });

                entity.HasMany<Customer>(c => c.Customers)
                      .WithOne(c => c.Company);

                entity.HasMany<CustomerGroup>(c => c.CustomerGroups)
                      .WithOne(cg => cg.Company);
            });

            modelBuilder.Entity<CustomerGroup>(entity => {
                entity.HasKey(cg => new { cg.CompanyID, cg.ID });

                entity.HasOne<Company>(cg => cg.Company)
                      .WithMany(c => c.CustomerGroups)
                      .HasForeignKey(c => c.CompanyID)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany<CustomerGroupCustomer>(cg => cg.CustomerGroupCustomers)
                      .WithOne(cgc => cgc.CustomerGroup);
            });

            modelBuilder.Entity<CustomerGroupCustomer>(entity => {
                entity.HasKey(cgc => new { cgc.CustomerGroupCustomerID });

                entity.HasIndex(cgc => cgc.CustomerGroupID).IsUnique();
                entity.HasIndex(cgc => cgc.CustomerID).IsUnique();

                entity.HasOne<Company>(cgc => cgc.Company)
                      .WithMany(c => c.CustomerGroupCustomers)
                      .HasForeignKey(cgc => cgc.CompanyID)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne<CustomerGroup>(cgc => cgc.CustomerGroup)
                      .WithMany(cg => cg.CustomerGroupCustomers)
                      .HasForeignKey(cgc => new { cgc.CompanyID, cgc.CustomerGroupID });

                entity.HasOne<Customer>(cgc => cgc.Customer)
                      .WithMany(c => c.CustomerGroupCustomers)
                      .HasForeignKey(cgc => new { cgc.CompanyID, cgc.CustomerID });
            });

            modelBuilder.Entity<Customer>(entity => {
                entity.HasKey(c => new { c.CompanyID, c.CustomerID });

                entity.HasOne<Company>(c => c.Company)
                      .WithMany(c => c.Customers)
                      .HasForeignKey(c => c.CompanyID);

                entity.HasMany<CustomerGroupCustomer>(c => c.CustomerGroupCustomers)
                      .WithOne(cgc => cgc.Customer);

                entity.HasMany<Division>(c => c.Divisions)
                      .WithOne(d => d.Customer);
            });

            modelBuilder.Entity<Division>(entity => {
                entity.HasKey(d => new { d.CompanyID, d.CustomerGroupID, d.CustomerID, d.DivisionID });

                entity.HasMany<Branch>(d => d.Branches)
                      .WithOne(b => b.Division);
            });

            modelBuilder.Entity<Branch>(entity => {
                entity.HasKey(b => new { b.CompanyID, b.CustomerGroupID, b.CustomerID, b.DivisionID, b.BranchID });

                entity.HasOne<Division>(b => b.Division)
                      .WithMany(d => d.Branches)
                      .HasForeignKey(b => new { b.CompanyID, b.CustomerGroupID, b.CustomerID, b.DivisionID });
            });

            modelBuilder.Entity<ApiKey>(entity => {
                entity.HasKey(a => new { a.ApiKeyID });
            });

            modelBuilder.Entity<WebApiStatistic>(entity => {
                entity.HasKey(wa => new { wa.WebApiStatisticID });
            });
        }
    }
}
