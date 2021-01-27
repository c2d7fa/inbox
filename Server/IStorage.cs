using System;
using System.Collections.Generic;
using System.Net;

namespace Inbox.Server {
    public interface IStorage {
        public IEnumerable<Message> Unread { get; }
        public void Create(Guid uuid, IPAddress author, string content);
        public void Read(Guid uuid);
    }
}
