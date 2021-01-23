using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Inbox
{
    public static class Static
    {
        [FunctionName("Static")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ExecutionContext executionContext,
            ILogger log)
        {
            if (!req.Query.ContainsKey("path")) {
              log.LogWarning("Got invalid request from client: No path given for static resource");
              return new BadRequestResult();
            }
            string path = req.Query["path"];

            string contentType = req.Query["type"].FirstOrDefault() ?? "text/plain";

            var staticRoot = Path.Combine(executionContext.FunctionAppDirectory, "static");
            var file = Path.Combine(staticRoot, path);

            if (!Path.GetFullPath(file).StartsWith(Path.GetFullPath(staticRoot) + Path.DirectorySeparatorChar)) {
                log.LogWarning($"Path '{path}' would escape root directory");
                return new BadRequestResult();
            }

            if (!File.Exists(file)) {
                log.LogWarning($"Tried to access non-existent static resource '{file}' at full path '{Path.GetFullPath(file)}'");
                return new NotFoundResult();
            }

            log.LogInformation($"Returning static resource '{Path.GetFullPath(file)}'");

            return new FileContentResult(File.ReadAllBytes(file), contentType);
        }
    }
}
