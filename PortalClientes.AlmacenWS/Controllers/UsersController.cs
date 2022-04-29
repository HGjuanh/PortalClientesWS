using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using PortalClientes.AlmacenWS.Contexts;
using PortalClientes.AlmacenWS.Helpers;
using PortalClientes.AlmacenWS.Models;
using PortalClientes.AlmacenWS.Models.Errors;

namespace PortalClientes.AlmacenWS.Controllers {
    [ApiKeyAuth]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        private readonly AppDbContext _appDbContext;

        public UsersController(AppDbContext appDbContext) {
            this._appDbContext = appDbContext;
        }

        //GET api/<controller>
        [HttpGet("GetStructuredUsers")]
        public async Task<ActionResult<List<StructuredUser>>> GetStructuredUsers() {
            List<StructuredUser> structuredUsers = new List<StructuredUser>();

            List<IdentityUser> users = _appDbContext.Users.AsNoTracking().ToList();
            foreach (IdentityUser user in users) {
                StructuredUser structuredUser = StructuredUsers.GetStructuredCustomerUser(user: user);
                if (structuredUser == null) { continue; }

                structuredUsers.Add(structuredUser);
            }

            if (!structuredUsers.Any()) {
                await LogRequest(code: 404, message: "Lista de Usuarios solicitados está vacía");

                return NotFound(new ApiResponse(statusCode: 404, message: $"Users not found"));
            }

            await LogRequest(code: 200, message: "Estructuras de Usuarios solicitados encontradas satisfactoriamente");

            return Ok(new UsersOkResponse(result: structuredUsers));
        }

        //GET api/<controller>
        [HttpGet("GetStructuredUser")]
        [HttpPost("GetStructuredUser")]
        public async Task<ActionResult<StructuredUser>> GetStructuredUserByUserName(GetGetStructuredUserByUsernameModel structuredUserModel) {
            IdentityUser user = _appDbContext.Users.AsNoTracking().FirstOrDefault(u => u.UserName.Equals(structuredUserModel.UserName));
            if (user == null) {
                await LogRequest(code: 404, message: "Usuario solicitado no encontrado en catalogo");

                if (structuredUserModel.UserName == null) {
                    return NotFound(new ApiResponse(statusCode: 404, message: $"User not found without UserName parameter"));
                } else {
                    return NotFound(new ApiResponse(statusCode: 404, message: $"User not found with username {structuredUserModel.UserName}"));
                }
            }

            StructuredUser structuredUser = StructuredUsers.GetStructuredCustomerUser(user: user);
            if (structuredUser == null) {
                await LogRequest(code: 404, message: "Estructura del Usuario solicitado no encontrado en catalogo");

                return NotFound(new ApiResponse(statusCode: 404, message: $"User not found with username {structuredUserModel.UserName}"));
            }

            await LogRequest(code: 200, message: "Estructura del Usuario solicitado encontrado satisfactoriamente");

            return Ok(new UserOkResponse(result: structuredUser));
        }

        private async Task LogRequest(int code, string message) {
            string headerJson = Helper.HeaderDictionaryToString(HttpContext.Request.Headers);
            string requestHost = HttpContext.Request.Host.Value;
            string requestURL = HttpContext.Request.Path.Value;
            string requestContentType = HttpContext.Request.ContentType;
            HttpContext.Request.Headers.TryGetValue("Token", out var secretKey);

            string body = "";

            Request.EnableBuffering();
            Request.Body.Position = 0;
            using (var reader = new StreamReader(Request.Body, encoding: Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
                Request.Body.Position = 0;
            }

            var controllerName = ControllerContext.ActionDescriptor.ControllerName;
            var actionName = ControllerContext.ActionDescriptor.ActionName;

            await Statistics.CreateWebApiStatistic(controller: controllerName, action: actionName, host: requestHost, url: requestURL, contentType: requestContentType,
                                                   headers: headerJson, body: body,
                                                   statusCode: code, statusMessage: message, secretKey: secretKey);
        }
    }
}
