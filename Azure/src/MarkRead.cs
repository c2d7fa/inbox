using System;
using Inbox.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.Cosmos.Table;

namespace Inbox.Azure
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
            var messages = new Messages(new AzureTable(unreadMessagesTable), new AzureTable(readMessagesTable));

            if (!Authentication.IsAuthenticated(req, new AzureTable(authenticationTable))) {
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

            messages.MarkRead(Guid.Parse(uuid));

            return HttpHelper.FinalResponse(req);
        }
    }
}
