using Inbox.Core.TableStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Inbox.Core.Api {
    public class Share {
        private readonly ILogger log;
        private readonly UnreadMessages unreadMessages;

        public Share(ITable unread, ILogger log) {
            this.log = log;
            this.unreadMessages = new UnreadMessages(unread);
        }

        public IActionResult Respond(HttpRequest req) {
            string message =
                HttpHelper.GetForm(req, "url") ??
                HttpHelper.GetForm(req, "text") ??
                HttpHelper.GetForm(req, "title") ??
                "";

            if (!(req.HttpContext.Connection.RemoteIpAddress is { } author)) {
                log.LogError("Unable to get IP address of request!");
                return new StatusCodeResult(500);
            }

            unreadMessages.Insert(author, message);

            return new ContentResult {
                ContentType = "text/plain",
                Content = "Received message: " + message,
            };
        }
    }
}
