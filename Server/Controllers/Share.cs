using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging.Abstractions;
using Api = Inbox.Core.Api;

namespace Inbox.Server.Controllers {
    [ApiController]
    [Route("/api/Share")]
    public class Share : ControllerBase {
        [HttpPost]
        public IActionResult Get([FromServices] CloudTableClient client) {
            var unread = new AzureTable(client.GetTableReference("UnreadMessages"));

            var api = new Api.Share(unread, NullLogger.Instance);
            return api.Respond(Request);
        }
    }
}
