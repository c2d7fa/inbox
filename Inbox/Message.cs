using System;
using System.Net;

namespace Inbox {
    public class Message {
        public Guid Uuid { get; private set; }
        public DateTime Created { get; private set; }
        public IPAddress Author { get; private set; }
        public string Content { get; private set; }

        public Message(Guid uuid, DateTime created, IPAddress author, string content) {
            this.Uuid = uuid;
            this.Created = created;
            this.Author = author;
            this.Content = content;
        }
    }
}
