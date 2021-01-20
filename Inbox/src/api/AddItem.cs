using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using System.Net;
using Microsoft.Azure.Cosmos.Table;

using Inbox.TableStorage;

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
            var entity = new Entity(author.ToString(), Guid.NewGuid().ToString());
            entity.Set("Created", DateTime.UtcNow);
            entity.Set("Content", content);
            new AzureTable(unreadMessagesTable).Insert(entity);
            log.LogInformation("Successfully inserted message.");

            if (HttpHelper.HandlePageRedirect(req)) {
                return new EmptyResult();
            }

            return new OkResult();
        }
    }
}
