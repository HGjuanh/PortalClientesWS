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
    [DebuggerDisplay("Customer: {Name}")]
    public class Customer {
        public string CustomerID { get; set; }

        public string CompanyID { get; set; }

        public int CustomerNum { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Email { get; set; }

        public string SalesExecutiveID { get; set; }

        public string OperationsExecutiveID { get; set; }

        public string SupervisorID { get; set; }

        public string ARExecutiveID { get; set; }

        public string GroupID { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<CustomerGroupCustomer> CustomerGroupCustomers { get; set; }

        [NotMapped]
        public virtual CustomerGroup CustomerGroup { get; set; }
        
        public virtual ICollection<Division> Divisions { get; set; }

        [NotMapped]
        public virtual ICollection<Branch> Branches { get; set; }
    }

    public static class Customers {
        public static List<Customer> GetCustomers(string companyID, List<UserCustomer> userCustomers) {
            return Companies.GetCompany(companyID: companyID, userCustomers: userCustomers).Customers.ToList();
        }

        public static Customer GetCustomer(string companyID, string customerID, List<UserCustomer> userCustomers) {
            return (GetCustomers(companyID: companyID, userCustomers: userCustomers)).Where(c => c.CompanyID.Equals(companyID) &&
                                                                                                 c.CustomerID.Equals(customerID)).FirstOrDefault();
        }

        public static List<Customer> GetCustomersByGroup(string companyID, string customerGroupID, string customerID) {
            var HcoContext = new HencoDbContext();

            List<Customer> customers = HcoContext.Customers.AsNoTracking().Include(c => c.Company).Where(c => c.CompanyID.Equals(companyID) &&
                                                                                                              c.CustomerID.Equals(customerID)).ToList();

            if (customers == null) {
                customers = new List<Customer>();
            } else {
                foreach (Customer customer in customers) {
                    customer.Divisions = Divisions.GetDivisionsByCustomer(companyID: customer.CompanyID, customerGroupID: customerGroupID, customerID: customer.CustomerID);
                }
            }

            return customers;
        }
    }

    [DebuggerDisplay("Customer: {Name}")]
    public class CustomerWS {
        public string CompanyID { get; set; }

        public string CustomerID { get; set; }

        public int CustomerNum { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Email { get; set; }
    }
}
