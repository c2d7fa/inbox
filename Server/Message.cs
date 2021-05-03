using System;
using System.Net;
using System.Web;

namespace Inbox.Server {
    public class Message {
        public Guid Uuid { get; }
        public DateTime Created { get; }
        public IPAddress Author { get; }
        public string Content { get; }
        public bool IsUrgent { get; }

        public Message(Guid uuid, DateTime created, IPAddress author, string content, bool isUrgent) {
            Uuid = uuid;
            Created = created;
            Author = author;
            Content = content;
            IsUrgent = isUrgent;
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
