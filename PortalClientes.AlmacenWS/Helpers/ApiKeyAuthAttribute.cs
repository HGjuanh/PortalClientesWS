using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PortalClientes.AlmacenWS.Contexts;
using PortalClientes.AlmacenWS.Models;

namespace PortalClientes.AlmacenWS.Helpers {
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter {
        private const string ApiKeyHeaderName = "Token";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
            //<---- Crear tabla para guardar consultar y regristros de consumos para estadisticas
            string headerJson = Helper.HeaderDictionaryToString(context.HttpContext.Request.Headers);
            string requestHost = context.HttpContext.Request.Host.Value;
            string requestURL = context.HttpContext.Request.Path.Value;
            string requestContentType = context.HttpContext.Request.ContentType;

            var controllerName = ((ControllerBase)context.Controller)
                .ControllerContext.ActionDescriptor.ControllerName;
            var actionName = ((ControllerBase)context.Controller)
               .ControllerContext.ActionDescriptor.ActionName;

            string body = Helper.ActionArgumentsToString(context.ActionArguments);

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potencialApiKey)) {
                await Statistics.CreateWebApiStatistic(controller: controllerName, action: actionName, host: requestHost, url: requestURL, contentType: requestContentType, headers: headerJson, body: body,
                                                       statusCode: 406, statusMessage: "No se ha enviado el token en la consulta", secretKey: potencialApiKey);

                context.Result = new UnauthorizedResult();
                return;
            }

            var HcoContext = new HencoDbContext();
            ApiKey apiKey = HcoContext.ApiKeys.FirstOrDefault(a => a.SecretKey.Equals(potencialApiKey));
            if (apiKey == null) {
                await Statistics.CreateWebApiStatistic(controller: controllerName, action: actionName, host: requestHost, url: requestURL, contentType: requestContentType, headers: headerJson, body: body,
                                                       statusCode: 403, statusMessage: "No existe la llave secreta en el catalogo", secretKey: potencialApiKey);

                context.Result = new UnauthorizedResult();
                return;
            }

            if (!apiKey.Active) {
                await Statistics.CreateWebApiStatistic(controller: controllerName, action: actionName, host: requestHost, url: requestURL, contentType: requestContentType, headers: headerJson, body: body,
                                                       statusCode: 401, statusMessage: "No existe la llave secreta en el catalogo", secretKey: potencialApiKey);
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}
