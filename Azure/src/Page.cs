using System.Collections.Generic;
using System.IO;
using Inbox.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.Cosmos.Table;
using Scriban;
using Scriban.Parsing;
using Scriban.Syntax;

namespace Inbox.Azure
{
    public static class Page
    {
        [FunctionName("Page")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Table("UnreadMessages")] CloudTable unreadMessagesTable,
            [Table("Authentication")] CloudTable authenticationTable,
            ExecutionContext executionContext,
            ILogger log)
        {
            var unreadMessages = new UnreadMessages(new AzureTable(unreadMessagesTable));

            if (!req.Query.ContainsKey("path")) {
              log.LogWarning("Got invalid request from client: No path given for page");
              return new BadRequestResult();
            }
            string path = req.Query["path"];

            var staticRoot = Path.Combine(executionContext.FunctionAppDirectory, "static");
            var file = Path.Combine(staticRoot, path + ".sbnhtml");

            if (!Path.GetFullPath(file).StartsWith(Path.GetFullPath(staticRoot) + Path.DirectorySeparatorChar)) {
                log.LogWarning($"Path '{path}' would escape root directory");
                return new BadRequestResult();
            }

            if (!File.Exists(file)) {
                log.LogWarning($"Tried to access non-existent page '{file}' at full path '{Path.GetFullPath(file)}'");
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
                        memoized.Add("messages", unreadMessages.All);
                    }
                    value = memoized["messages"];
                    return true;
                } else if (variable.Name == "authenticated") {
                    if (!memoized.ContainsKey("authenticated")) {
                        memoized.Add("authenticated", Authentication.IsAuthenticated(req, new AzureTable(authenticationTable)));
                    }
                    value = memoized["authenticated"];
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
    }
}
