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

using Scriban;

namespace Inbox
{
    public static class AllItemsPage
    {
        [FunctionName("AllItemsPage")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Table("UnreadMessages")] CloudTable unreadMessagesTable,
            ILogger log)
        {
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

            foreach (var message in messages) {
                log.LogInformation("Message: " + message.Content);
            }

            var data = new {
                Messages = messages,
            };

            var response = new ContentResult();
            response.ContentType = "text/html";
            response.Content = Template.Parse(File.ReadAllText("static/list.sbnhtml")).Render(data);
            return response;
        }
    }
}
