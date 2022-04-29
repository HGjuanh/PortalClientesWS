using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace PortalClientes.AlmacenWS.Helpers {
    public static class Helper {
        public static string HeaderDictionaryToString(IHeaderDictionary dict) {
            Dictionary<string, object> result = new Dictionary<string, object>();

            foreach (StringValues keys in dict.Keys) {
                result.Add(key: keys, value: dict[keys]);
            }

            return JsonConvert.SerializeObject(result, Formatting.None);
        }

        public static string ActionArgumentsToString(IDictionary<string, object> result) {
            return JsonConvert.SerializeObject(result, Formatting.None);
        }
    }
}
