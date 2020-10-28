using Microsoft.AspNetCore.Http;

namespace Inbox {
  public static class HttpHelper {
    public static bool HandlePageRedirect(HttpRequest req) {
      if (GetForm(req, "page") != null) {
        var response = req.HttpContext.Response;
        response.StatusCode = 303;
        response.Headers.Add("Location", "/api/Page?path=" + GetForm(req, "page"));
        return true;
      } else {
        return false;
      }
    }

    public static string GetForm(HttpRequest req, string key) {
      var values = req.Form[key];
      if (values.Count == 1) {
          return values[0];
      } else {
          return null;
      }
    }
  }
}
