using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalClientes.AlmacenWS.Models {
    public class CustomerGroupCustomer {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CustomerGroupCustomerID { get; set; }

        public string CompanyID { get; set; }

        public string CustomerGroupID { get; set; }

        public string CustomerID { get; set; }

        public virtual Company Company { get; set; }

        public virtual CustomerGroup CustomerGroup { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
