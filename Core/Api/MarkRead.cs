using System;
using Inbox.Core.TableStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Inbox.Core.Api {
    public class MarkRead {
        private readonly ILogger log;
        private readonly Messages messages;
        private readonly ITable authentication;

        public MarkRead(ITable unread, ITable read, ITable authentication, ILogger log) {
            this.log = log;
            this.messages = new Messages(unread, read);
            this.authentication = authentication;
        }

        public IActionResult Respond(HttpRequest req) {
            if (!Authentication.IsAuthenticated(req, authentication)) {
                log.LogInformation("User was not authenticated when getting all tables");
                return new UnauthorizedResult();
            }

            if (!req.Query.ContainsKey("message")) {
                log.LogWarning("Got invalid request from client: No message UUID");
                return new BadRequestResult();
            }

            string uuid = req.Query["message"];
            messages.MarkRead(Guid.Parse(uuid));

            return HttpHelper.FinalResponse(req);
        }
    }
}
