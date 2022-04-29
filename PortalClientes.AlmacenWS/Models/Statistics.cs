using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PortalClientes.AlmacenWS.Contexts;

namespace PortalClientes.AlmacenWS.Models {
    public class Statistics {
        public static async Task CreateWebApiStatistic(string controller, string action, string host, string url, string contentType, string headers, string body,
                                                       int statusCode = 200, string statusMessage = "", string secretKey = "") {
            HencoDbContext HcoContext = new HencoDbContext();

            var strategy = HcoContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {
                var transaction = HcoContext.Database.BeginTransaction();
                try {
                    WebApiStatistic webApiStatistic = new WebApiStatistic {
                        WebApiStatisticID = Guid.NewGuid().ToString(),
                        Controller = controller,
                        Action = action,
                        Host = host,
                        URL = url,
                        ContentType = contentType,
                        Headers = headers,
                        Body = body,
                        StatusCode = statusCode,
                        StatusMessage = statusMessage,
                        SecretApiKey = secretKey,
                        CreatedDate = DateTime.Now
                    };

                    while (HcoContext.WebApiStatistics.AsNoTracking().Where(wa => wa.WebApiStatisticID.Equals(webApiStatistic.WebApiStatisticID)).Any()) {
                        webApiStatistic.WebApiStatisticID = Guid.NewGuid().ToString();
                    }

                    await HcoContext.WebApiStatistics.AddAsync(webApiStatistic);

                    HcoContext.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    transaction.Rollback();
                }
            });
        }
    }
}
