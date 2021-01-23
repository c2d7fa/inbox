using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers {
    [ApiController]
    [Route("/api/AllItems")]
    public class AllItems : ControllerBase {
        [HttpGet]
        public object Get() {
            // [TODO]
            return new List<object>();
        }
    }
}
