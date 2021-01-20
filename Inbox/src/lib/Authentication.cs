using Inbox.TableStorage;
using Microsoft.AspNetCore.Http;

namespace Inbox {
  public static class Authentication {
    private static bool IsValidToken(string token, ITable authenticationTable) {
      return authenticationTable.HasRow(token);
    }

    public static bool IsAuthenticated(HttpRequest request, ITable authenticationTable) {
      return IsAuthenticatedByCookie(request, authenticationTable) || IsAuthenticatedByQuery(request, authenticationTable);
    }

    private static bool IsAuthenticatedByQuery(HttpRequest request, ITable authenticationTable) {
      var token = request.Query["token"];
      if (token.Count != 1) return false;
      return IsValidToken(token[0], authenticationTable);
    }

    private static bool IsAuthenticatedByCookie(HttpRequest request, ITable authenticationTable) {
      var token = request.Cookies["InboxAuthenticationToken"];
      return token != null && IsValidToken(token, authenticationTable);
    }
  }
}
