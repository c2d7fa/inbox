namespace Inbox.Server {
    public interface IAuthentication {
        public bool IsValidToken(string token);
    }
}
