using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalClientes.AlmacenWS.Contexts;

namespace PortalClientes.AlmacenWS.Models {
    public class UserConfig {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly HencoDbContext _HcoContext = new HencoDbContext();

        private List<UserCustomer> _userCustomers;
        private UserConfig _relatedUserConfig;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string UserConfigID { get; set; }

        [Display(Name = "Logo")]
        public byte[] Logo { get; set; }

        [Display(Name = "Logo BN")]
        public byte[] LogoBco { get; set; }

        [Display(Name = "Modo Oscuro")]
        public bool DarkMode { get; set; }

        [Display(Name = "Idioma")]
        public string Language { get; set; }

        public bool HasShipments { get; set; }

        public bool HasQuotations { get; set; }

        public bool HasBillingAccounts { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Compañía Predeterminado.")]
        [Display(Name = "Compañía Predeterminada")]
        public string DefaultCompanyID { get; set; }

        [Display(Name = "Grupo de Clientes Predeterminado")]
        public string DefaultCustomerGroupID { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un Cliente Predeterminado.")]
        [Display(Name = "Cliente Predeterminado")]
        public string DefaultCustomerID { get; set; }

        [Display(Name = "División Predeterminado")]
        public string DefaultDivisionID { get; set; }

        [Display(Name = "Sucursal Predeterminado")]
        public string DefaultBranchID { get; set; }

        public bool Status { get; set; }

        public string UserID { get; set; }

        public virtual IdentityUser User { get; set; }

        [NotMapped]
        public virtual List<UserCustomer> UserCustomers {
            get {
                if (_userCustomers == null) {
                    _userCustomers = _context.UserCustomers.AsNoTracking().Where(uc => uc.UserConfigID.Equals(UserConfigID)).ToList();

                    if (_userCustomers.Count > 0) {
                        Customers = new List<Customer>();

                        foreach (UserCustomer userCustomer in _userCustomers) {
                            userCustomer.UserConfig = this;

                            userCustomer.Company = _HcoContext.Companies.AsNoTracking().Where(c => c.CompanyID.Equals(userCustomer.CompanyID)).FirstOrDefault();
                            userCustomer.CustomerGroup = null;
                            userCustomer.Customer = null;
                            userCustomer.Division = null;
                            userCustomer.Branch = null;

                            if (userCustomer.CustomerGroupID != "") {
                                userCustomer.CustomerGroup = _HcoContext.CustomerGroups.AsNoTracking().Where(cg => cg.CompanyID.Equals(userCustomer.CompanyID) &&
                                                                                                                   cg.ID.Equals(userCustomer.CustomerGroupID)).FirstOrDefault();
                            }

                            if (userCustomer.CustomerID != null && userCustomer.CustomerID != "") {
                                if (userCustomer.CustomerID != "TODOS") {
                                    userCustomer.Customer = _HcoContext.Customers.AsNoTracking().Where(c => c.CompanyID.Equals(userCustomer.CompanyID) &&
                                                                                                            c.CustomerID.Equals(userCustomer.CustomerID)).FirstOrDefault();

                                    Customers.Add(userCustomer.Customer);
                                } else {
                                    List<CustomerGroupCustomer> groupCustomers = _HcoContext.CustomerGroupCustomers.AsNoTracking().Where(cgc => cgc.CompanyID.Equals(userCustomer.CompanyID) &&
                                                                                                                                                cgc.CustomerGroupID.Equals(userCustomer.CustomerGroupID)).ToList();
                                    Customers = (groupCustomers.Select(cgc => _HcoContext.Customers.AsNoTracking().Where(c => c.CompanyID.Equals(cgc.CompanyID) &&
                                                                                                                              c.CustomerID.Equals(cgc.CustomerID)).FirstOrDefault())).ToList();
                                }
                            }

                            if (userCustomer.DivisionID != "" && userCustomer.DivisionID != "TODAS") {
                                userCustomer.Division = _HcoContext.Divisions.AsNoTracking().Where(d => d.CompanyID.Equals(userCustomer.CompanyID) &&
                                                                                                        d.CustomerGroupID.Equals(userCustomer.CustomerGroupID) &&
                                                                                                        d.CustomerID.Equals(userCustomer.CustomerID) &&
                                                                                                        d.DivisionID.Equals(userCustomer.DivisionID)).FirstOrDefault();
                            }

                            if (userCustomer.BranchID != "" && userCustomer.BranchID != "TODAS") {
                                userCustomer.Branch = _HcoContext.Branches.AsNoTracking().Where(b => b.CompanyID.Equals(userCustomer.CompanyID) &&
                                                                                                     b.CustomerGroupID.Equals(userCustomer.CustomerGroupID) &&
                                                                                                     b.CustomerID.Equals(userCustomer.CustomerID) &&
                                                                                                     b.DivisionID.Equals(userCustomer.DivisionID) &&
                                                                                                     b.BranchID.Equals(userCustomer.BranchID)).FirstOrDefault();
                            }
                        }
                    }
                }

                return _userCustomers;
            }

            set => _userCustomers = value;
        }

        public string RelatedUserConfigID { get; set; }

        public virtual UserConfig RelatedUserConfig {
            get {
                if (_relatedUserConfig == null && RelatedUserConfigID != null && RelatedUserConfigID != "") {
                    _relatedUserConfig = _context.UserConfigs.AsNoTracking().Include(u => u.User)
                                                                            .Where(u => u.UserConfigID.Equals(RelatedUserConfigID)).FirstOrDefault();
                }

                return _relatedUserConfig;
            }

            set => _relatedUserConfig = value;
        }

        public virtual ICollection<UserConfig> RelatedUserConfigs { get; set; }

        [NotMapped]
        public virtual Company DefaultCompany { get; set; }

        [NotMapped]
        public virtual CustomerGroup DefaultCustomerGroup { get; set; }

        [NotMapped]
        public virtual Customer DefaultCustomer { get; set; }

        [NotMapped]
        public virtual Division DefaultDivision { get; set; }

        [NotMapped]
        public virtual Branch DefaultBranch { get; set; }

        [NotMapped]
        public virtual List<Customer> Customers { get; set; }
    }

    public static class UserConfigs {
        public static UserConfig GetUserConfig(IdentityUser user) {
            AppDbContext dbContext = new AppDbContext();

            UserConfig userConfig = dbContext.UserConfigs.AsNoTracking().Include(u => u.User)
                                                                        .Where(u => u.UserID.Equals(user.Id)).FirstOrDefault();

            return userConfig;
        }
    }
}
