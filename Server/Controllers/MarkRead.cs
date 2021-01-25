using System;
using Inbox.Server.TableStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Inbox.Server.Controllers {
    [ApiController]
    [Route("/api/MarkRead")]
    public class MarkRead : ControllerBase {
        [HttpPost]
        public IActionResult Get([FromServices] IAuthentication authentication, [FromServices] IStorage storage) {
            var log = NullLogger.Instance;

            if (!authentication.IsRequestAuthenticated(Request)) {
                log.LogInformation("User was not authenticated when getting all tables");
                return new UnauthorizedResult();
            }

            if (!Request.Query.ContainsKey("message")) {
                log.LogWarning("Got invalid request from client: No message UUID");
                return new BadRequestResult();
            }

            string uuid = Request.Query["message"];
            storage.Read(Guid.Parse(uuid));

            return HttpHelper.FinalResponse(Request);
        }
    }
}
