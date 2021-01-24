using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using Api = Inbox.Core.Api;

namespace Inbox.Azure {
    public static class MarkRead {
        [FunctionName("MarkRead")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            [Table("UnreadMessages")] CloudTable unreadMessagesTable,
            [Table("ReadMessages")] CloudTable readMessagesTable,
            [Table("Authentication")] CloudTable authenticationTable,
            ILogger log
        ) {
            var api = new Api.MarkRead(
                new AzureTable(unreadMessagesTable),
                new AzureTable(readMessagesTable),
                new AzureTable(authenticationTable),
                log
            );
            return api.Respond(req);
        }
    }
}
