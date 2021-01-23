using Inbox.Core.TableStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Inbox.Core.Api {
    public class AddItem {
        private readonly UnreadMessages unreadMessages;
        private readonly ILogger log;

        public AddItem(ITable unreadMessagesTable, ILogger log) {
            this.unreadMessages = new UnreadMessages(unreadMessagesTable);
            this.log = log;
        }

        public IActionResult Respond(HttpRequest req) {
            if (!(HttpHelper.GetForm(req, "content") is { } content)) {
                log.LogWarning("Could not get content from message.");
                return new BadRequestResult();
            }

            if (!(req.HttpContext.Connection.RemoteIpAddress is { } author)) {
                log.LogError("Unable to get IP address of request!");
                return new StatusCodeResult(500);
            }

            unreadMessages.Insert(author, content);

            return HttpHelper.FinalResponse(req);
        }
    }

    internal static class HttpHelper {
        public static IActionResult FinalResponse(HttpRequest req) {
            if (!(GetForm(req, "page") is { } page))
                return new OkResult();

            var response = req.HttpContext.Response;
            response.StatusCode = 303;
            response.Headers.Add("Location", "/" + page);
            return new EmptyResult();
        }

        public static string? GetForm(HttpRequest req, string key) {
            var values = req.Form[key];
            return values.Count == 1 ? values[0] : null;
        }
    }
}
