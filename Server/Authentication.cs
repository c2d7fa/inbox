using Microsoft.AspNetCore.Http;

namespace Inbox.Server {
    public static class AuthenticationExtensions {
        public static bool IsRequestAuthenticated(this IAuthentication authentication, HttpRequest request) {
            return IsAuthenticatedByCookie() || IsAuthenticatedByQuery();

            bool IsAuthenticatedByCookie() {
                var token = request.Cookies["InboxAuthenticationToken"];
                return token != null && authentication.IsValidToken(token);
            }

            bool IsAuthenticatedByQuery() {
                var token = request.Query["token"];
                return token.Count == 1 && authentication.IsValidToken(token[0]);
            }
        }
    }
}
