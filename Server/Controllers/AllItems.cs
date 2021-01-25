using System.Linq;
using Inbox.Server.TableStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Inbox.Server.Controllers {
    [ApiController]
    [Route("/api/AllItems")]
    public class AllItems : ControllerBase {
        [HttpGet]
        public IActionResult Get([FromServices] CloudTableClient client, [FromServices] IStorage storage) {
            var log = NullLogger.Instance;
            var authentication = new AzureTable(client.GetTableReference("Authentication"));

            if (!Authentication.IsAuthenticated(Request, authentication)) {
                log.LogInformation("User was not authenticated when getting all messages.");
                return new ForbidResult();
            }

            return new JsonResult(storage.Unread.Select(message => new {
                message.Uuid,
                message.Created,
                message.Content,
                message.HtmlContent,
                Author = message.Author.ToString(),
            }));
        }
    }
}
