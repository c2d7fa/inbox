using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using Api = Inbox.Core.Api;

namespace Inbox.Azure {
    public static class Share {
        [FunctionName("Share")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            [Table("UnreadMessages")] CloudTable unreadMessagesTable,
            ILogger log
        ) {
            var api = new Api.Share(new AzureTable(unreadMessagesTable), log);
            return api.Respond(req);
        }
    }
}
