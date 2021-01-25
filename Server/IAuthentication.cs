using Inbox.Server.TableStorage;

namespace Inbox.Server {
    public interface IAuthentication {
        public bool IsValidToken(string token);
    }

    public class TableAuthentication : IAuthentication {
        private readonly ITable table;

        public TableAuthentication(ITable table) {
            this.table = table;
        }

        public bool IsValidToken(string token) {
            return table.HasRow(token);
        }
    }
}
