using System.Web.Http;
using Inbox.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;

namespace Inbox.Azure {
    public static class AddItem {
        [FunctionName("AddItem")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            [Table("UnreadMessages")] CloudTable unreadMessagesTable,
            ILogger log) {
            var unreadMessages = new UnreadMessages(new AzureTable(unreadMessagesTable));

            if (!(HttpHelper.GetForm(req, "content") is { } content)) {
                log.LogWarning("Could not get content from message.");
                return new BadRequestResult();
            }

            if (!(req.HttpContext.Connection.RemoteIpAddress is { } author)) {
                log.LogError("Unable to get IP address of request!");
                return new InternalServerErrorResult();
            }

            unreadMessages.Insert(author, content);

            if (HttpHelper.HandlePageRedirect(req)) {
                return new EmptyResult();
            }

            return new OkResult();
        }
    }
}
