using Microsoft.AspNetCore.Http;

namespace Inbox.Azure {
  public static class HttpHelper {
    public static bool HandlePageRedirect(HttpRequest req) {
      if (GetForm(req, "page") != null) {
        var response = req.HttpContext.Response;
        response.StatusCode = 303;
        response.Headers.Add("Location", "/" + GetForm(req, "page"));
        return true;
      } else {
        return false;
      }
    }

    public static string? GetForm(HttpRequest req, string key) {
      var values = req.Form[key];
      return values.Count == 1 ? values[0] : null;
    }
  }
}
