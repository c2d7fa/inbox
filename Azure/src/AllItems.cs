using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using Api = Inbox.Core.Api;

namespace Inbox.Azure {
    public static class AllItems {
        [FunctionName("AllItems")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            [Table("UnreadMessages")] CloudTable unreadMessagesTable,
            [Table("Authentication")] CloudTable authenticationTable,
            ILogger log
        ) {
            var api = new Api.AllItems(new AzureTable(unreadMessagesTable), new AzureTable(authenticationTable), log);
            return api.Respond(req);
        }
    }
}
