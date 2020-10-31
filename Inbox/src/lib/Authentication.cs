using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Table;

namespace Inbox {
  public static class Authentication {
    public static bool IsValidToken(string token, CloudTable authenticationTable) {
      return authenticationTable.ExecuteQuery(new TableQuery().Where(TableQuery.GenerateFilterCondition("RowKey", "eq", token))).Count() > 0;
    }

    public static bool IsAuthenticated(HttpRequest request, CloudTable authenticationTable) {
      return IsAuthenticatedByCookie(request, authenticationTable) || IsAuthenticatedByQuery(request, authenticationTable);
    }

    public static bool IsAuthenticatedByQuery(HttpRequest request, CloudTable authenticationTable) {
      var token = request.Query["token"];
      if (token.Count != 1) return false;
      return IsValidToken(token[0], authenticationTable);
    }

    public static bool IsAuthenticatedByCookie(HttpRequest request, CloudTable authenticationTable) {
      var token = request.Cookies["InboxAuthenticationToken"];
      if (token == null) return false;
      return IsValidToken(token, authenticationTable);
    }
  }
}
