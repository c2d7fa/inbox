using Inbox.Core.TableStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging.Abstractions;
using Api = Inbox.Core.Api;

namespace Inbox.Server.Controllers {
    [ApiController]
    [Route("/api/AllItems")]
    public class AllItems : ControllerBase {
        [HttpGet]
        public IActionResult Get([FromServices] CloudTableClient client) {
            var authentication = new AzureTable(client.GetTableReference("Authentication"));
            var unread = new AzureTable(client.GetTableReference("UnreadMessages"));

            var api = new Api.AllItems(unread, authentication, NullLogger.Instance);
            return api.Respond(Request);
        }
    }
}
