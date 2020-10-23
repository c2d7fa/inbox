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
            log.LogInformation("Reading body of request...");
            var content = new StreamReader(req.Body).ReadToEnd();
            log.LogInformation($"Got string with {content.Length} characters.");

            var author = req.HttpContext.Connection.RemoteIpAddress;
            log.LogInformation($"It looks like this message came from '{author}'.");

            log.LogInformation("Inserting message into database...");
            var entity = new DynamicTableEntity(author.ToString(), Guid.NewGuid().ToString(), "", new Dictionary<string, EntityProperty>{
                { "Created", new EntityProperty(DateTime.UtcNow) },
                { "Content", new EntityProperty(content) },
            });
            unreadMessagesTable.Execute(TableOperation.Insert(entity));
            log.LogInformation("Successfully inserted message.");

            return new OkResult();
        }
    }
}
