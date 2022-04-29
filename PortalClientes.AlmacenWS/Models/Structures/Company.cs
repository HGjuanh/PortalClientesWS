using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortalClientes.AlmacenWS.Contexts;

namespace PortalClientes.AlmacenWS.Models {
    [DebuggerDisplay("Company: {Name}")]
    public class Company {
        public string CompanyID { get; set; }

        public string Name { get; set; }

        public string RFC { get; set; }

        public bool Active { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }

        public virtual ICollection<CustomerGroupCustomer> CustomerGroupCustomers { get; set; }

        public virtual ICollection<CustomerGroup> CustomerGroups { get; set; }

        [NotMapped]
        public virtual ICollection<Division> Divisions { get; set; }

        [NotMapped]
        public virtual ICollection<Branch> Branches { get; set; }

    }

    public static class Companies {
        public static List<Company> GetCompanies(List<UserCustomer> userCustomers) {
            List<Company> companies = new List<Company>();

            using (var HcoContext = new HencoDbContext()) {
                foreach (Company company in HcoContext.Companies.AsNoTracking().ToList()) {
                    company.CustomerGroups = new List<CustomerGroup>();
                    company.Customers = new List<Customer>();
                    company.Divisions = new List<Division>();
                    company.Branches = new List<Branch>();

                    List<UserCustomer> uCustomers = userCustomers.Where(uc => uc.CompanyID.Equals(company.CompanyID)).ToList();

                    foreach (UserCustomer userCustomer in uCustomers) {
                        if (userCustomer.CustomerGroupID == "") {
                            Customer customer = HcoContext.Customers.AsNoTracking().Where(c => c.CompanyID.Equals(userCustomer.CompanyID) &&
                                                                                               c.CustomerID.Equals(userCustomer.CustomerID)).FirstOrDefault();
                            if (customer != null) {
                                company.Customers.Add(customer);
                            }
                        } else {
                            CustomerGroup customerGroup = HcoContext.CustomerGroups.AsNoTracking().Include(cg => cg.CustomerGroupCustomers)
                                                                                                  .Where(cg => cg.CompanyID.Equals(userCustomer.CompanyID) &&
                                                                                                               cg.ID.Equals(userCustomer.CustomerGroupID)).FirstOrDefault();
                            customerGroup.Customers = CustomerGroups.GetCustomersByGroup(customerGroup.ID, customerGroup.CustomerGroupCustomers);
                            
                            if (!company.CustomerGroups.Where(cg => cg.CompanyID.Equals(customerGroup.CompanyID) &&
                                                                    cg.ID.Equals(customerGroup.ID)).Any()) {
                                company.CustomerGroups.Add(customerGroup);
                            }

                            if (userCustomer.CustomerID == "TODOS") {
                                customerGroup.Customers.ForEach(c => { if (!company.Customers.Where(cs => cs.CustomerID.Equals(c.CustomerID)).Any()) company.Customers.Add(c); });
                            } else {
                                if (!company.Customers.Where(c => c.CompanyID.Equals(userCustomer.CompanyID) &&
                                                                  c.CustomerID.Equals(userCustomer.CustomerID)).Any()) {
                                    company.Customers.Add(customerGroup.Customers.Where(c => c.CompanyID.Equals(userCustomer.CompanyID) &&
                                                                                             c.CustomerID.Equals(userCustomer.CustomerID)).FirstOrDefault());
                                }
                            }

                            List<Division> divisions = new List<Division>();
                            foreach (Customer customer in company.Customers) {
                                divisions.AddRange(Divisions.GetDivisionsByCustomer(companyID: customer.CompanyID,
                                                                                    customerGroupID: customerGroup.ID,
                                                                                    customerID: customer.CustomerID));
                            }

                            if (userCustomer.DivisionID == "TODAS") {
                                divisions.ForEach(d => { if (!company.Divisions.Where(dn => dn.CompanyID.Equals(d.CompanyID) &&
                                                                                            dn.CustomerGroupID.Equals(d.CustomerGroupID) &&
                                                                                            dn.CustomerID.Equals(d.CustomerID) &&
                                                                                            dn.DivisionID.Equals(d.DivisionID)).Any()) company.Divisions.Add(d); });
                            } else {
                                if (!company.Divisions.Where(d => d.DivisionID.Equals(userCustomer.DivisionID)).Any()) {
                                    company.Divisions.Add(divisions.Where(d => d.DivisionID.Equals(userCustomer.DivisionID)).FirstOrDefault());
                                }
                            }

                            List<Branch> branches = new List<Branch>();
                            foreach (Division division in company.Divisions) {
                                branches.AddRange(Branches.GetBranchesByDivision(companyID: division.CompanyID,
                                                                                customerGroupID: division.CustomerGroupID,
                                                                                customerID: division.CustomerID,
                                                                                divisionID: division.DivisionID));
                            }

                            if (userCustomer.BranchID == "TODAS") {
                                branches.ForEach(b => { if (!company.Branches.Where(bc => bc.CompanyID.Equals(b.CompanyID) &&
                                                                                          bc.CustomerGroupID.Equals(b.CustomerGroupID) &&
                                                                                          bc.CustomerID.Equals(b.CustomerID) &&
                                                                                          bc.DivisionID.Equals(b.DivisionID) &&
                                                                                          bc.BranchID.Equals(b.BranchID)).Any()) company.Branches.Add(b); });
                            } else {
                                if (!company.Branches.Where(b => b.BranchID.Equals(userCustomer.BranchID)).Any()) {
                                    company.Branches.Add(branches.Where(b => b.BranchID.Equals(userCustomer.BranchID)).FirstOrDefault());
                                }
                            }
                        }
                    }

                    if (company.CustomerGroups.Any()) {
                        company.CustomerGroups = company.CustomerGroups.OrderBy(cg => cg.Description).ToList();
                    }

                    if (company.Customers.Any()) {
                        company.Customers = company.Customers.OrderBy(c => c.Name).ToList();
                    }

                    if (company.Divisions.Any()) {
                        company.Divisions = company.Divisions.OrderBy(d => d.CompanyID)
                                                             .ThenBy(d => d.CustomerGroupID)
                                                             .ThenBy(d => d.CustomerID)
                                                             .ThenBy(d => d.Description).ToList();
                    }

                    if (company.Branches.Any()) {
                        company.Branches = company.Branches.OrderBy(b => b.CompanyID)
                                                           .ThenBy(b => b.CustomerGroupID)
                                                           .ThenBy(b => b.CustomerID)
                                                           .ThenBy(b => b.DivisionID)
                                                           .ThenBy(b => b.Description).ToList();
                    }

                    companies.Add(company);
                }
            }

            return companies;
        }

        public static Company GetCompany(string companyID, List<UserCustomer> userCustomers) {
            return (GetCompanies(userCustomers: userCustomers)).Where(c => c.CompanyID.Equals(companyID)).FirstOrDefault();
        }
    }

    [DebuggerDisplay("Company: {Name}")]
    public class CompanyWS {
        public string CompanyID { get; set; }

        public string Name { get; set; }

        public string RFC { get; set; }

        public bool Active { get; set; }
    }
}
