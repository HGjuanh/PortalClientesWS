using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalClientes.AlmacenWS.Contexts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PortalClientes.AlmacenWS.Models {

    [DebuggerDisplay("User: {Name} ({Username})")]
    public class StructuredUser {
        public string Username { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Language { get; set; }

        public bool DarkMode { get; set; }

        public byte[] Logo { get; set; }

        public byte[] LogoBco { get; set; }

        public virtual CompanyWS DefaultCompany { get; set; }

        public virtual CustomerGroupWS DefaultCustomerGroup { get; set; }

        public virtual CustomerWS DefaultCustomer { get; set; }

        public virtual DivisionWS DefaultDivision { get; set; }

        public virtual BranchWS DefaultBranch { get; set; }

        public virtual List<CompanyWS> Companies { get; set; }

        public virtual List<CustomerGroupWS> CustomerGroups { get; set; }

        public virtual List<CustomerWS> Customers { get; set; }

        public virtual List<DivisionWS> Divisions { get; set; }

        public virtual List<BranchWS> Branches { get; set; }
    }

    public class GetGetStructuredUserByUsernameModel {
        public string UserName { get; set; }
    }

    public static class StructuredUsers {
        private static readonly AppDbContext AppContext = new AppDbContext();

        public static StructuredUser GetStructuredCustomerUser(IdentityUser user) {
            CustomerGroupWS defaultCustomerGroup = null;
            CustomerWS defaultCustomer = null;
            DivisionWS defaultDivision = null;
            BranchWS defaultBranch = null;

            List<CustomerGroupWS> customerGroups = new List<CustomerGroupWS>();
            List<CustomerWS> customers = new List<CustomerWS>();
            List<DivisionWS> divisions = new List<DivisionWS>();
            List<BranchWS> branches = new List<BranchWS>();

            IdentityRole roleAsCustomer = AppContext.Roles.AsNoTracking().FirstOrDefault(r => r.Name.Equals("Admin"));
            IdentityRole roleAsAdmin = AppContext.Roles.AsNoTracking().FirstOrDefault(r => r.Name.Equals("Root"));
            IdentityRole roleAsHencoUser = AppContext.Roles.AsNoTracking().FirstOrDefault(r => r.Name.Equals("HencoAdmin"));

            bool validUser = false;

            UserData userData = AppContext.UserDatas.AsNoTracking().Include(u => u.User).FirstOrDefault(u => u.UserID.Equals(user.Id));
            if (userData == null) { return null; }

            UserConfig userConfig = AppContext.UserConfigs.AsNoTracking().Include(u => u.User).FirstOrDefault(u => u.UserID.Equals(user.Id));
            if (userConfig == null) { return null; }

            var userRole = AppContext.UserRoles.AsNoTracking().FirstOrDefault(ur => ur.UserId.Equals(user.Id));

            if (userRole != null) {
                if (userRole.RoleId.Equals(roleAsCustomer.Id)) { validUser = true; }

                if (userRole.RoleId.Equals(roleAsAdmin.Id)) { validUser = true; }

                if (userRole.RoleId.Equals(roleAsHencoUser.Id)) { validUser = true; }
            }

            if (!validUser) { return null; }

            Company company = Companies.GetCompany(companyID: userConfig.DefaultCompanyID, userCustomers: userConfig.UserCustomers.ToList());

            CompanyWS defaultCompany = new CompanyWS {
                CompanyID = company.CompanyID,
                Name = company.Name,
                RFC = company.RFC,
                Active = company.Active
            };

            if (userConfig.DefaultCustomerGroupID != "") {
                CustomerGroup customerGroup = company.CustomerGroups.Where(cg => cg.CompanyID.Equals(userConfig.DefaultCompanyID) &&
                                                                                 cg.ID.Equals(userConfig.DefaultCustomerGroupID)).FirstOrDefault();
                if (customerGroup != null) {
                    defaultCustomerGroup = new CustomerGroupWS {
                        CompanyID = customerGroup.CompanyID,
                        CustomerGroupID = customerGroup.ID,
                        Description = customerGroup.Description,
                        Active = customerGroup.Active
                    };
                }
            }

            if (userConfig.DefaultCustomerID != "") {
                Customer customer = company.Customers.Where(c => c.CompanyID.Equals(userConfig.DefaultCompanyID) &&
                                                                 c.CustomerID.Equals(userConfig.DefaultCustomerID)).FirstOrDefault();
                if (customer != null) {
                    defaultCustomer = new CustomerWS {
                        CompanyID = customer.CompanyID,
                        CustomerID = customer.CustomerID,
                        CustomerNum = customer.CustomerNum,
                        Name = customer.Name,
                        Email = customer.Email,
                        Address = customer.Address,
                        PostalCode = customer.PostalCode,
                        City = customer.City,
                        State = customer.State,
                        Country = customer.Country
                    };
                }
            }

            if (userConfig.DefaultDivisionID != "") {
                Division division = company.Divisions.Where(d => d.CompanyID.Equals(userConfig.DefaultCompanyID) &&
                                                                 d.CustomerGroupID.Equals(userConfig.DefaultCustomerGroupID) &&
                                                                 d.CustomerID.Equals(userConfig.DefaultCustomerID) &&
                                                                 d.DivisionID.Equals(userConfig.DefaultDivisionID)).FirstOrDefault();
                if (division != null) {
                    defaultDivision = new DivisionWS {
                        CompanyID = division.CompanyID,
                        CustomerGroupID = division.CustomerGroupID,
                        CustomerID = division.CustomerID,
                        DivisionID = division.DivisionID,
                        Description = division.Description,
                        Active = division.Active
                    };
                }
            }

            if (userConfig.DefaultBranchID != "") {
                Branch branch = company.Branches.Where(b => b.CompanyID.Equals(userConfig.DefaultCompanyID) &&
                                                            b.CustomerGroupID.Equals(userConfig.DefaultCustomerGroupID) &&
                                                            b.CustomerID.Equals(userConfig.DefaultCustomerID) &&
                                                            b.DivisionID.Equals(userConfig.DefaultDivisionID) &&
                                                            b.BranchID.Equals(userConfig.DefaultBranchID)).FirstOrDefault();
                if (branch != null) {
                    defaultBranch = new BranchWS {
                        CompanyID = branch.CompanyID,
                        CustomerGroupID = branch.CustomerGroupID,
                        CustomerID = branch.CustomerID,
                        DivisionID = branch.DivisionID,
                        BranchID = branch.BranchID,
                        Description = branch.Description,
                        Active = branch.Active
                    };
                }
            }

            if (company.CustomerGroups.Any()) {
                company.CustomerGroups.ToList().ForEach(customerGroup => customerGroups.Add(new CustomerGroupWS {
                    CompanyID = customerGroup.CompanyID,
                    CustomerGroupID = customerGroup.ID,
                    Description = customerGroup.Description,
                    Active = customerGroup.Active
                }));
            }

            if (company.Customers.Any()) {
                company.Customers.ToList().ForEach(customer => customers.Add(new CustomerWS {
                    CompanyID = customer.CompanyID,
                    CustomerID = customer.CustomerID,
                    CustomerNum = customer.CustomerNum,
                    Name = customer.Name,
                    Address = customer.Address,
                    City = customer.City,
                    State = customer.State,
                    PostalCode = customer.PostalCode,
                    Country = customer.Country,
                    Email = customer.Email
                }));
            }

            if (company.Divisions.Any()) {
                company.Divisions.ToList().ForEach(division => divisions.Add(new DivisionWS {
                    CompanyID = division.CompanyID,
                    CustomerGroupID = division.CustomerGroupID,
                    CustomerID = division.CustomerID,
                    DivisionID = division.DivisionID,
                    Description = division.Description,
                    Active = division.Active
                }));
            }

            if (company.Branches.Any()) {
                company.Branches.ToList().ForEach(branch => branches.Add(new BranchWS {
                    CompanyID = branch.CompanyID,
                    CustomerGroupID = branch.CustomerGroupID,
                    CustomerID = branch.CustomerID,
                    DivisionID = branch.DivisionID,
                    BranchID = branch.BranchID,
                    Description = branch.Description,
                    Active = branch.Active
                }));
            }

            return new StructuredUser {
                Username = user.UserName,
                Name = userData.FullName,

                Email = user.Email,
                Phone = user.PhoneNumber,
                Language = userConfig.Language,
                DarkMode = userConfig.DarkMode,
                Logo = userConfig.Logo,
                LogoBco = userConfig.LogoBco,

                DefaultCompany = defaultCompany,
                DefaultCustomerGroup = defaultCustomerGroup,
                DefaultCustomer = defaultCustomer,
                DefaultDivision = defaultDivision,
                DefaultBranch = defaultBranch,

                Companies = new List<CompanyWS>() { defaultCompany },
                CustomerGroups = customerGroups,
                Customers = customers,
                Divisions = divisions,
                Branches = branches
            };
        }
    }

}
