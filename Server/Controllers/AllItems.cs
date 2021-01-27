using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Inbox.Server.Controllers {
    [ApiController]
    [Route("/api/AllItems")]
    public class AllItems : ControllerBase {
        [HttpGet]
        public IActionResult Get([FromServices] IAuthentication authentication, [FromServices] IStorage storage) {
            var log = NullLogger.Instance;

            if (!authentication.IsRequestAuthenticated(Request)) {
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
