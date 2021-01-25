using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Inbox.Server.Controllers {
    [ApiController]
    [Route("/api/Share")]
    public class Share : ControllerBase {
        [HttpPost]
        public IActionResult Get([FromServices] CloudTableClient client) {
            var log = NullLogger.Instance;
            var unread = new UnreadMessages(new AzureTable(client.GetTableReference("UnreadMessages")));

            string message =
                HttpHelper.GetForm(Request, "url") ??
                HttpHelper.GetForm(Request, "text") ??
                HttpHelper.GetForm(Request, "title") ??
                "";

            if (!(Request.HttpContext.Connection.RemoteIpAddress is { } author)) {
                log.LogError("Unable to get IP address of request!");
                return new StatusCodeResult(500);
            }

            unread.Insert(author, message);

            return new ContentResult {
                ContentType = "text/plain",
                Content = "Received message: " + message,
            };
        }
    }
}
