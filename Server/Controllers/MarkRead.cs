using Inbox.Core.TableStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging.Abstractions;
using Api = Inbox.Core.Api;

namespace Inbox.Server.Controllers {
    [ApiController]
    [Route("/api/MarkRead")]
    public class MarkRead : ControllerBase {
        [HttpPost]
        public IActionResult Get([FromServices] CloudTableClient client) {
            var authentication = new AzureTable(client.GetTableReference("Authentication"));
            var unread = new AzureTable(client.GetTableReference("UnreadMessages"));
            var read = new AzureTable(client.GetTableReference("ReadMessages"));

            var api = new Api.MarkRead(unread, read, authentication, NullLogger.Instance);
            return api.Respond(Request);
        }
    }
}
