using System;
using System.Collections.Generic;
using System.Net;

namespace Inbox.Server {
    public interface IStorage {
        public IEnumerable<Message> Unread { get; }
        public void Create(IPAddress author, string content);
        public void Read(Guid uuid);
    }

    public class MirroredStorage : IStorage {
        private IStorage primary;
        private IStorage mirror;

        public MirroredStorage(IStorage primary, IStorage mirror) {
            this.primary = primary;
            this.mirror = mirror;
        }

        public IEnumerable<Message> Unread => primary.Unread;

        public void Create(IPAddress author, string content) {
            primary.Create(author, content);
            mirror.Create(author, content);
        }

        public void Read(Guid uuid) {
            primary.Read(uuid);
            mirror.Read(uuid);
        }
    }
}
