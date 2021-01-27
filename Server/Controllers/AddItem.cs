using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Inbox.Server.Controllers {
    [ApiController]
    [Route("/api/AddItem")]
    public class AddItem : ControllerBase {
        [HttpPost]
        public IActionResult Get([FromServices] IStorage storage) {
            var log = NullLogger.Instance;

            if (!(HttpHelper.GetForm(Request, "content") is { } content)) {
                log.LogWarning("Could not get content from message.");
                return new BadRequestResult();
            }

            if (!(HttpContext.Connection.RemoteIpAddress is { } author)) {
                log.LogError("Unable to get IP address of request!");
                return new StatusCodeResult(500);
            }

            storage.Create(Guid.NewGuid(), author, content);

            return HttpHelper.FinalResponse(Request);
        }
    }
}
