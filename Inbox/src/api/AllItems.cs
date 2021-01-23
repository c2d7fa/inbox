using Inbox.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
            ILogger log) {
            var unreadMessages = new UnreadMessages(new AzureTable(unreadMessagesTable));

            if (!Authentication.IsAuthenticated(req, new AzureTable(authenticationTable))) {
                log.LogInformation("User was not authenticated when getting all tables");
                return new UnauthorizedResult();
            }

            return new JsonResult(unreadMessages.All, new JsonSerializerSettings().WithIPAddress());
        }
    }
}
