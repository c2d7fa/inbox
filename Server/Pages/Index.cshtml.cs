using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Inbox.Server.TableStorage;

namespace Inbox.Server.Pages {
    public class IndexModel : PageModel {
        private readonly ILogger<IndexModel> log;
        private readonly ITable authentication;

        public bool IsAuthenticated => Authentication.IsAuthenticated(HttpContext.Request, authentication);
        public UnreadMessages UnreadMessages { get; private set; }

        public IndexModel(ILogger<IndexModel> log, CloudTableClient client) {
            this.authentication = new AzureTable(client.GetTableReference("Authentication"));
            this.log = log;
            UnreadMessages = new UnreadMessages(new AzureTable(client.GetTableReference("UnreadMessages")));
        }

        public void OnGet() { }
    }
}
