using Inbox.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;

namespace Inbox
{
    public static class AddItem
    {
        [FunctionName("AddItem")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Table("UnreadMessages")] CloudTable unreadMessagesTable,
            ILogger log)
        {
            var unreadMessages = new UnreadMessages(new AzureTable(unreadMessagesTable));

            log.LogInformation("Reading new message content...");
            var content = HttpHelper.GetForm(req, "content");
            if (content == null) {
                log.LogWarning("Could not get content from message.");
                return new BadRequestResult();
            }
            log.LogInformation($"Got string with {content.Length} characters.");

            var author = req.HttpContext.Connection.RemoteIpAddress;
            log.LogInformation($"It looks like this message came from '{author}'.");

            log.LogInformation("Inserting message into database...");
            unreadMessages.Insert(author, content);
            log.LogInformation("Successfully inserted message.");

            if (HttpHelper.HandlePageRedirect(req)) {
                return new EmptyResult();
            }

            return new OkResult();
        }
    }
}
