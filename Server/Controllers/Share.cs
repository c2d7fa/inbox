using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Inbox.Server.Controllers {
    [ApiController]
    [Route("/api/Share")]
    public class Share : ControllerBase {
        [HttpPost]
        public IActionResult Get([FromServices] IStorage storage) {
            var log = NullLogger.Instance;

            string message =
                HttpHelper.GetForm(Request, "url") ??
                HttpHelper.GetForm(Request, "text") ??
                HttpHelper.GetForm(Request, "title") ??
                "";

            if (!(Request.HttpContext.Connection.RemoteIpAddress is { } author)) {
                log.LogError("Unable to get IP address of request!");
                return new StatusCodeResult(500);
            }

            storage.Create(Guid.NewGuid(), author, message);

            return new ContentResult {
                ContentType = "text/plain",
                Content = "Received message: " + message,
            };
        }
    }
}
