using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inbox.Azure {
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
