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

            var entities = unreadMessagesTable.ExecuteQuery(new TableQuery());
            foreach (var entity in entities) {
                var uuid = Guid.Parse(entity.RowKey);
                var created_ = entity.Properties["Created"].DateTime;
                var author = IPAddress.Parse(entity.PartitionKey);
                var content = entity.Properties["Content"].StringValue;

                if (created_ is DateTime created) {
                    messages.Add(new Message(uuid, created, author, content));
                }
            }

            return new JsonResult(messages, new JsonSerializerSettings().WithIPAddress());
        }
    }
}
