using System;
using Inbox.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Inbox.Server.Controllers {
    [ApiController]
    [Route("/api/MarkRead")]
    public class MarkRead : ControllerBase {
        [HttpPost]
        public IActionResult Get([FromServices] CloudTableClient client) {
            var log = NullLogger.Instance;
            var authentication = new AzureTable(client.GetTableReference("Authentication"));
            var messages = new Messages(
                new AzureTable(client.GetTableReference("UnreadMessages")),
                new AzureTable(client.GetTableReference("ReadMessages"))
            );

            if (!Authentication.IsAuthenticated(Request, authentication)) {
                log.LogInformation("User was not authenticated when getting all tables");
                return new UnauthorizedResult();
            }

            if (!Request.Query.ContainsKey("message")) {
                log.LogWarning("Got invalid request from client: No message UUID");
                return new BadRequestResult();
            }

            string uuid = Request.Query["message"];
            messages.MarkRead(Guid.Parse(uuid));

            return HttpHelper.FinalResponse(Request);
        }
    }
}
