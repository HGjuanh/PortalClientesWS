using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PortalClientes.AlmacenWS.Models {
    public class WebApiStatistic {
        [Key()]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string WebApiStatisticID { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Headers { get; set; }

        public string Host { get; set; }

        public string URL { get; set; }

        public string ContentType { get; set; }

        public string Body { get; set; }

        public int StatusCode { get; set; }

        public string StatusMessage { get; set; }

        public string SecretApiKey { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}
