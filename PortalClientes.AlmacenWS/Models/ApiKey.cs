using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PortalClientes.AlmacenWS.Models {
    public class ApiKey {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ApiKeyID { get; set; }

        public string SecureKey { get; set; }

        public string SecretKey { get; set; }

        public string ResposibleName { get; set; }

        public string ResposibleEmail { get; set; }

        public string ResposiblePhone { get; set; }

        public bool Active { get; set; }
    }
}
