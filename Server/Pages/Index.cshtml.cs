﻿using Inbox.Core.TableStorage;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Inbox.Core;

namespace Server.Pages {
    public class IndexModel : PageModel {
        private readonly ILogger<IndexModel> log;
        private readonly ITable authentication;

        public bool IsAuthenticated => Authentication.IsAuthenticated(HttpContext.Request, authentication);

        public IndexModel(ILogger<IndexModel> log, CloudTableClient client) {
            this.authentication = new AzureTable(client.GetTableReference("Authentication"));
            this.log = log;
        }

        public void OnGet() { }
    }
}
