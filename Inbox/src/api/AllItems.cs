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
using Inbox.TableStorage;
using Microsoft.Azure.Cosmos.Table;

namespace Inbox
{
    public static class AllItems
    {
        [FunctionName("AllItems")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Table("UnreadMessages")] CloudTable unreadMessagesTable,
            [Table("Authentication")] CloudTable authenticationTable,
            ILogger log)
        {
            if (!Authentication.IsAuthenticated(req, new AzureTable(authenticationTable))) {
                log.LogInformation("User was not authenticated when getting all tables");
                return new UnauthorizedResult();
            }

            var messages = new List<Message>();

            var entities = new AzureTable(unreadMessagesTable).AllEntities;
            foreach (var entity in entities) {
                var uuid = Guid.Parse(entity.Row);
                var author = IPAddress.Parse(entity.Partition);
                var created = entity.Property("Created");//.DateTime;
                var content = entity.Property("Content");//.StringValue;

                if (created is DateTime createdDateTime && content is string contentString) {
                    messages.Add(new Message(uuid, createdDateTime, author, contentString));
                } else {
                    log.LogWarning("Couldn't read entity with UUID " + entity.Row);
                }
            }

            return new JsonResult(messages, new JsonSerializerSettings().WithIPAddress());
        }
    }
}
