namespace Inbox.Server.TableStorage {
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
