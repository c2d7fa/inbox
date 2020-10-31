using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Table;

namespace Inbox {
  public static class Authentication {
    public static bool IsValidToken(string token, CloudTable authenticationTable) {
      return authenticationTable.ExecuteQuery(new TableQuery().Where(TableQuery.GenerateFilterCondition("RowKey", "eq", token))).Count() > 0;
    }

    public static bool IsAuthenticated(HttpRequest request, CloudTable authenticationTable) {
      var token = request.Cookies["InboxAuthenticationToken"];
      if (token == null) return false;
      return IsValidToken(token, authenticationTable);
    }
  }
}
