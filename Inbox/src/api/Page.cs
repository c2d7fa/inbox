using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.Cosmos.Table;
using Scriban;
using Scriban.Parsing;
using Scriban.Syntax;

namespace Inbox
{
    public static class Page
    {
        [FunctionName("Page")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Table("UnreadMessages")] CloudTable unreadMessagesTable,
            ILogger log)
        {
            if (!req.Query.ContainsKey("path")) {
              log.LogWarning("Got invalid request from client: No path given for page");
              return new BadRequestResult();
            }
            string path = req.Query["path"];

            var file = Path.Combine("static", path + ".sbnhtml");

            if (!Path.GetFullPath(file).StartsWith(Path.GetFullPath("static") + Path.DirectorySeparatorChar)) {
                log.LogWarning($"Path '{path}' would escape root directory");
                return new BadRequestResult();
            }

            if (!File.Exists(file)) {
                log.LogWarning($"Tried to access non-existent page '{file}'");
                return new NotFoundResult();
            }

            var context = new TemplateContext();

            // We use memoization because we may want to access the variables
            // many times, without executing side-effects each time.
            var memoized = new Dictionary<string, object>();
            context.TryGetVariable += (TemplateContext context, SourceSpan span, ScriptVariable variable, out object value) => {
                if (variable.Name == "messages") {
                    if (!memoized.ContainsKey("messages")) {
                        log.LogInformation("Getting messages because they haven't been cached yet");
                        memoized.Add("messages", GetMessages(unreadMessagesTable, log));
                    }
                    value = memoized["messages"];
                    return true;
                } else {
                    value = null;
                    return false;
                }
            };

            var response = new ContentResult();
            response.ContentType = "text/html";
            response.Content = Template.Parse(File.ReadAllText(file)).Render(context);
            return response;
        }

        private static List<Message> GetMessages(CloudTable unreadMessagesTable, ILogger log) {
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

            return messages;
        }
    }
}
