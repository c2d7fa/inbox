﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Server.Pages {
    public class IndexModel : PageModel {
        private readonly ILogger<IndexModel> log;

        public IndexModel(ILogger<IndexModel> log) {
            this.log = log;
        }

        public void OnGet() { }
    }
}
