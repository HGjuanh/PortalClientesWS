using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PortalClientes.AlmacenWS.Models {
    public class UserCustomer {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string UserCustomerID { get; set; }

        public string UserConfigID { get; set; }

        public string CompanyID { get; set; }

        public string CustomerGroupID { get; set; }

        public string CustomerID { get; set; }

        public string DivisionID { get; set; }

        public string BranchID { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual UserConfig UserConfig { get; set; }

        [NotMapped]
        public virtual Company Company { get; set; }

        [NotMapped]
        public virtual CustomerGroup CustomerGroup { get; set; }

        [NotMapped]
        public virtual Customer Customer { get; set; }

        [NotMapped]
        public virtual Division Division { get; set; }

        [NotMapped]
        public virtual Branch Branch { get; set; }
    }
}
