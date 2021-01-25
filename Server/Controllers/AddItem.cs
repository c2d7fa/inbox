using Inbox.Server.TableStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Inbox.Server.Controllers {
    [ApiController]
    [Route("/api/AddItem")]
    public class AddItem : ControllerBase {
        [HttpPost]
        public IActionResult Get([FromServices] CloudTableClient client) {
            var log = NullLogger.Instance;
            var unread = new UnreadMessages(new AzureTable(client.GetTableReference("UnreadMessages")));

            if (!(HttpHelper.GetForm(Request, "content") is { } content)) {
                log.LogWarning("Could not get content from message.");
                return new BadRequestResult();
            }

            if (!(HttpContext.Connection.RemoteIpAddress is { } author)) {
                log.LogError("Unable to get IP address of request!");
                return new StatusCodeResult(500);
            }

            unread.Insert(author, content);

            return HttpHelper.FinalResponse(Request);
        }
    }
}
