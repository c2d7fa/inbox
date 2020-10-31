using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.Cosmos.Table;

namespace Inbox
{
    public static class MarkRead
    {
        [FunctionName("MarkRead")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Table("UnreadMessages")] CloudTable unreadMessagesTable,
            [Table("ReadMessages")] CloudTable readMessagesTable,
            [Table("Authentication")] CloudTable authenticationTable,
            ILogger log)
        {
            if (!Authentication.IsAuthenticated(req, authenticationTable)) {
                log.LogInformation("User was not authenticated when getting all tables");
                return new UnauthorizedResult();
            }

            log.LogInformation("Reading message UUID...");
            if (!req.Query.ContainsKey("message")) {
              log.LogWarning("Got invalid request from client: No message UUID");
              return new BadRequestResult();
            }
            string uuid = req.Query["message"];
            log.LogInformation($"Marking message '{uuid}' as read...");

            if (GetEntityByRowKey(unreadMessagesTable, uuid) is DynamicTableEntity entity) {
              log.LogInformation("Message found. Moving to ReadMessages table...");
              readMessagesTable.Execute(TableOperation.Insert(entity));
              log.LogInformation("Deleting message from UnreadMessages...");
              unreadMessagesTable.Execute(TableOperation.Delete(GetEntityByRowKey(unreadMessagesTable, uuid)));
            } else {
              log.LogWarning("Message not found. Ignoring.");
            }

            if (HttpHelper.HandlePageRedirect(req)) {
              return new EmptyResult();
            }

            return new OkResult();
        }

        private static DynamicTableEntity GetEntityByRowKey(CloudTable table, string rowKey) {
          var matchingEntities = new List<DynamicTableEntity>(table.ExecuteQuery(new TableQuery().Where(
            TableQuery.GenerateFilterCondition("RowKey", "eq", rowKey)
          )));

          if (matchingEntities.Count == 1) {
            return matchingEntities[0];
          } else {
            return null;
          }
        }
    }
}
