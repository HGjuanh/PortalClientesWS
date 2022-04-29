using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortalClientes.AlmacenWS.Contexts;

namespace PortalClientes.AlmacenWS.Models {
    [DebuggerDisplay("Branch: {Description}")]
    public class Branch {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string BranchID { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }

        public string CompanyID { get; set; }

        public string CustomerGroupID { get; set; }

        public string CustomerID { get; set; }

        public string DivisionID { get; set; }

        [NotMapped]
        public virtual Company Company { get; set; }

        [NotMapped]
        public virtual CustomerGroup CustomerGroup { get; set; }

        [NotMapped]
        public virtual Customer Customer { get; set; }

        [NotMapped]
        public virtual Division Division { get; set; }
    }

    public static class Branches {
        public static List<Branch> GetBranchesByCompany(string companyID, List<UserCustomer> userCustomers) {
            return Companies.GetCompany(companyID: companyID, userCustomers: userCustomers).Branches.ToList();
        }

        public static Branch GetBrancheByCompany(string companyID, string customerGroupID, string divisionID, string branchID, List<UserCustomer> userCustomers) {
            return (GetBranchesByCompany(companyID: companyID, userCustomers: userCustomers)).Where(b => b.CompanyID.Equals(companyID) &&
                                                                                                         b.CustomerGroupID.Equals(customerGroupID) &&
                                                                                                         b.DivisionID.Equals(divisionID) &&
                                                                                                         b.BranchID.Equals(branchID)).FirstOrDefault();
        }

        public static List<Branch> GetBranchesByDivision(string companyID, string customerGroupID, string customerID, string divisionID) {
            var HcoContext = new HencoDbContext();

            List<Branch> branches = HcoContext.Branches.AsNoTracking().Where(b => b.CompanyID.Equals(companyID) &&
                                                                                  b.CustomerGroupID.Equals(customerGroupID) &&
                                                                                  b.CustomerID.Equals(customerID) &&
                                                                                  b.DivisionID.Equals(divisionID)).ToList();
            if (branches == null) {
                branches = new List<Branch>();
            } else {
                foreach (Branch branch in branches) {
                    branch.Company = HcoContext.Companies.AsNoTracking().Where(c => c.CompanyID.Equals(branch.CompanyID)).FirstOrDefault();

                    branch.CustomerGroup = HcoContext.CustomerGroups.AsNoTracking().Where(cg => cg.CompanyID.Equals(branch.CompanyID) &&
                                                                                                cg.ID.Equals(branch.CustomerGroupID)).FirstOrDefault();

                    branch.Customer = HcoContext.Customers.AsNoTracking().Where(c => c.CompanyID.Equals(branch.CompanyID) &&
                                                                                     c.CustomerID.Equals(branch.CustomerID)).FirstOrDefault();

                    branch.Division = HcoContext.Divisions.AsNoTracking().Where(d => d.CompanyID.Equals(branch.CompanyID) &&
                                                                                     d.CustomerGroupID.Equals(branch.CustomerGroupID) &&
                                                                                     d.CustomerID.Equals(branch.CustomerID) &&
                                                                                     d.DivisionID.Equals(branch.DivisionID)).FirstOrDefault();
                }

                branches = branches.OrderBy(b => b.Description).ToList();
            }

            return branches;
        }
    }

    [DebuggerDisplay("Branch: {Description}")]
    public class BranchWS {
        public string CompanyID { get; set; }

        public string CustomerGroupID { get; set; }

        public string CustomerID { get; set; }

        public string DivisionID { get; set; }

        public string BranchID { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }  }
}
