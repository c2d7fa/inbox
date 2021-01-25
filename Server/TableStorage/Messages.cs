using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Inbox.Server.TableStorage {
    public class UnreadMessages {
        private readonly ITable table;

        public UnreadMessages(ITable table) {
            this.table = table;
        }

        // [TODO] We should do some error checking here to make sure that we're not
        // returning corrupt data. Also, we may just straight up crash here if the data
        // is in a weird format.
        public IEnumerable<Message> All => table
            .AllEntities
            .SelectMany(entity => {
                var uuid = Guid.Parse(entity.Row);
                var author = IPAddress.Parse(entity.Partition);
                var created = entity.Property("Created");
                var content = entity.Property("Content");

                if (created is DateTime createdDateTime && content is string contentString) {
                    return new List<Message> {new Message(uuid, createdDateTime, author, contentString)};
                } else {
                    return new List<Message> { };
                }
            })
            .OrderBy(message => message.Created);

        public void Insert(IPAddress author, string content) {
            var entity = new Entity(author.ToString(), Guid.NewGuid().ToString());
            entity.Set("Created", DateTime.UtcNow);
            entity.Set("Content", content);
            table.Insert(entity);
        }
    }

    public class Messages : IStorage {
        private readonly ITable unread;
        private readonly ITable read;

        private readonly UnreadMessages unreadMessages;

        public Messages(ITable unread, ITable read) {
            this.unread = unread;
            this.read = read;
            this.unreadMessages = new UnreadMessages(unread);
        }

        public void MarkRead(Guid uuid) {
            var key = uuid.ToString();
            if (!(unread.Get(key) is { } entity)) return;
            read.Insert(entity);
            unread.Delete(key);
        }

        public IEnumerable<Message> Unread => unreadMessages.All;
        public void Create(IPAddress author, string content) => unreadMessages.Insert(author, content);
        public void Read(Guid uuid) => MarkRead(uuid);
    }
}
