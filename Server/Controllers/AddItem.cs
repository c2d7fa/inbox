using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging.Abstractions;
using Api = Inbox.Core.Api;

namespace Server.Controllers {
    [ApiController]
    [Route("/api/AddItem")]
    public class AddItem : ControllerBase {
        [HttpPost]
        public IActionResult Get([FromServices] CloudTableClient client) {
            var unread = new AzureTable(client.GetTableReference("UnreadMessages"));

            var api = new Api.AddItem(unread, NullLogger.Instance);
            return api.Respond(Request);
        }
    }
}
