using System;
using System.Net;
using System.Web;

namespace Inbox.Core {
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

        public string HtmlContent {
            get {
                if (Content.StartsWith("https://")) {
                    return $"<a href=\"{HttpUtility.HtmlAttributeEncode(Content)}\">{HttpUtility.HtmlEncode(Content)}</a>";
                } else {
                    return HttpUtility.HtmlEncode(Content);
                }
            }
        }
    }
}
