using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PortalClientes.AlmacenWS.Models.Errors {
    public class ApiResponse {
        public int StatusCode { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; }

        public ApiResponse(int statusCode, string message = null) {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        private static string GetDefaultMessageForStatusCode(int statusCode) {
            return statusCode switch {
                200 => "Response correctly",
                400 => "Bad Request",
                401 => "Unauthorized",
                404 => "Not found",
                500 => "An internal error occurred",
                _ => $"An unhandled error occurred. Code: {statusCode}",
            };
        }
    }

    public class UsersOkResponse : ApiResponse {
        public object StructuredUsers { get; }

        public UsersOkResponse(object result) : base(200) {
            StructuredUsers = result;
        }
    }

    public class UserOkResponse : ApiResponse {
        public object StructuredUser { get; }

        public UserOkResponse(object result) : base(200) {
            StructuredUser = result;
        }
    }
}
