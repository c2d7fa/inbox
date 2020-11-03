using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.Cosmos.Table;

namespace Inbox
{
  public static class Share
  {
    [FunctionName("Share")]
    public static IActionResult Run(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
      [Table("UnreadMessages")] CloudTable unreadMessagesTable,
      ILogger log)
    {
      string message = null;

      string url = HttpHelper.GetForm(req, "url");
      string text = HttpHelper.GetForm(req, "text");
      string title = HttpHelper.GetForm(req, "title");

      if (url != null && url != "") {
        log.LogInformation("Shared message with URL: " + url);
        message = url;
      } else if (text != null && text != "") {
        log.LogInformation("Shared message with text: " + text);
        message = text;
      } else if (title != null && title != "") {
        log.LogInformation("Shared message with title: " + title);
        message = title;
      }

      if (message == null) {
        log.LogWarning("Tried to share message, but it had no content; ignoring");
        return new BadRequestResult();
      }

      log.LogInformation($"Adding shared message with {message.Length} characters...");

      var author = req.HttpContext.Connection.RemoteIpAddress;
      log.LogInformation($"It looks like this message came from '{author}'.");

      log.LogInformation("Inserting message into database...");
      var entity = new DynamicTableEntity(author.ToString(), Guid.NewGuid().ToString(), "", new Dictionary<string, EntityProperty>{
          { "Created", new EntityProperty(DateTime.UtcNow) },
          { "Content", new EntityProperty(message) },
      });
      unreadMessagesTable.Execute(TableOperation.Insert(entity));

      var response = new ContentResult();
      response.ContentType = "text/plain";
      response.Content = "Sent message: " + message;
      return response;
    }
  }
}
