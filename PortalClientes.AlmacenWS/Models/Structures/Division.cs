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
    [DebuggerDisplay("Division: {Description}")]
    public class Division {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string DivisionID { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }

        public string CompanyID { get; set; }

        public string CustomerGroupID { get; set; }

        public string CustomerID { get; set; }

        [NotMapped]
        public virtual Company Company { get; set; }

        [NotMapped]
        public virtual CustomerGroup CustomerGroup { get; set; }

        [NotMapped]
        public virtual Customer Customer { get; set; }

        public virtual ICollection<Branch> Branches { get; set; }
    }

    public static class Divisions {
        public static List<Division> GetDivisionsByCompany(string companyID, List<UserCustomer> userCustomers) {
            return Companies.GetCompany(companyID: companyID, userCustomers: userCustomers).Divisions.ToList();
        }

        public static Division GetDivisionByCompany(string companyID, string customerGroupID, string divisionID, List<UserCustomer> userCustomers) {
            return (GetDivisionsByCompany(companyID: companyID, userCustomers: userCustomers)).Where(d => d.CompanyID.Equals(companyID) &&
                                                                                                          d.CustomerGroupID.Equals(customerGroupID) &&
                                                                                                          d.DivisionID.Equals(divisionID)).FirstOrDefault();
        }

        public static List<Division> GetDivisionsByCustomer(string companyID, string customerGroupID, string customerID) {
            var HcoContext = new HencoDbContext();

            List<Division> divisions = HcoContext.Divisions.AsNoTracking().Where(d => d.CompanyID.Equals(companyID) &&
                                                                                      d.CustomerGroupID.Equals(customerGroupID) &&
                                                                                      d.CustomerID.Equals(customerID)).ToList();
            if (divisions == null) {
                divisions = new List<Division>();
            } else {
                foreach (Division division in divisions) {
                    division.Company = HcoContext.Companies.AsNoTracking().Where(c => c.CompanyID.Equals(division.CompanyID)).FirstOrDefault();

                    division.CustomerGroup = HcoContext.CustomerGroups.AsNoTracking().Where(cg => cg.CompanyID.Equals(division.CompanyID) &&
                                                                                                  cg.ID.Equals(division.CustomerGroupID)).FirstOrDefault();

                    division.Customer = HcoContext.Customers.AsNoTracking().Where(c => c.CompanyID.Equals(division.CompanyID) &&
                                                                                       c.CustomerID.Equals(division.CustomerID)).FirstOrDefault();

                    division.Branches = Branches.GetBranchesByDivision(companyID: companyID, customerGroupID: customerGroupID, customerID: customerID, divisionID: division.DivisionID);
                }

                divisions = divisions.OrderBy(d => d.Description).ToList();
            }

            return divisions;
        }
    }

    [DebuggerDisplay("Division: {Description}")]
    public class DivisionWS {
        public string CompanyID { get; set; }

        public string CustomerGroupID { get; set; }

        public string CustomerID { get; set; }

        public string DivisionID { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }
    }
}
