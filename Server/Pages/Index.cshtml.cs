using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Inbox.Server.TableStorage;

namespace Inbox.Server.Pages {
    public class IndexModel : PageModel {
        private readonly ILogger<IndexModel> log;
        private readonly IAuthentication authentication;

        public bool IsAuthenticated => authentication.IsRequestAuthenticated(HttpContext.Request);
        public IStorage Messages { get; }

        public IndexModel(ILogger<IndexModel> log, IAuthentication authentication, IStorage storage) {
            this.log = log;
            this.authentication = authentication;
            Messages = storage;
        }

        public void OnGet() { }
    }
}
