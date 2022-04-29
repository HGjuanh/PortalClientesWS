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
    [DebuggerDisplay("Group: {Description}")]
    public class CustomerGroup {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }

        public string CompanyID { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<CustomerGroupCustomer> CustomerGroupCustomers { get; set; }

        [NotMapped]
        public virtual List<Customer> Customers { get; set; }
    }

    public static class CustomerGroups {
        private static readonly HencoDbContext _HcoContext = new HencoDbContext();

        public static List<CustomerGroup> GetCustomerGroupsByCompany(string companyID, List<UserCustomer> userCustomers) {
            return Companies.GetCompany(companyID: companyID, userCustomers: userCustomers).CustomerGroups.ToList();
        }

        public static CustomerGroup GetCustomerGroupByCompany(string companyID, string customerGroupID, List<UserCustomer> userCustomers) {
            return (GetCustomerGroupsByCompany(companyID: companyID, userCustomers: userCustomers)).Where(cg => cg.CompanyID.Equals(companyID) &&
                                                                                                                cg.ID.Equals(customerGroupID)).FirstOrDefault();
        }

        public static List<Customer> GetCustomersByGroup(string customerGroupID, ICollection<CustomerGroupCustomer> customerGroupCustomers) {
            List<Customer> customers = new List<Customer>();

            if (customerGroupCustomers != null && customerGroupCustomers.Any()) {
                foreach (Customer customer in from CustomerGroupCustomer cgc in customerGroupCustomers
                                              let customer = _HcoContext.Customers.AsNoTracking().Include(c => c.Company)
                                                                                                .Where(c => c.CompanyID.Equals(cgc.CompanyID) &&
                                                                                                            c.CustomerID.Equals(cgc.CustomerID)).FirstOrDefault()
                                            select customer) {
                    if (customer == null) { continue; }
                    
                    customer.Divisions = Divisions.GetDivisionsByCustomer(companyID: customer.CompanyID, customerGroupID: customerGroupID, customerID: customer.CustomerID);

                    customers.Add(customer);
                }

                if (customers.Any()) {
                    customers = customers.OrderBy(c => c.Name).ToList();
                }
            }

            return customers;
        }
    }

    [DebuggerDisplay("Group: {Description}")]
    public class CustomerGroupWS {
        public string CompanyID { get; set; }

        public string CustomerGroupID { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }
    }
}
