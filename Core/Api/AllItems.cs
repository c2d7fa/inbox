using System.Linq;
using Inbox.Core.TableStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Inbox.Core.Api {
    public class AllItems {
        private readonly UnreadMessages unreadMessages;
        private readonly ILogger log;
        private readonly ITable authenticationTable;

        public AllItems(ITable unreadMessagesTable, ITable authenticationTable, ILogger log) {
            unreadMessages = new UnreadMessages(unreadMessagesTable);
            this.authenticationTable = authenticationTable;
            this.log = log;
        }

        public IActionResult Respond(HttpRequest req) {
            if (!Authentication.IsAuthenticated(req, authenticationTable)) {
                log.LogInformation("User was not authenticated when getting all messages.");
                return new ForbidResult();
            }

            return new JsonResult(unreadMessages.All.Select(message => new {
                message.Uuid,
                message.Created,
                message.Content,
                message.HtmlContent,
                Author = message.Author.ToString(),
            }));
        }
    }
}
