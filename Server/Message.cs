using System;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;

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
                string LinkUrls(string content) {
                    var urlRegex = new Regex(@"(https)://\S+");
                    var html = urlRegex.Replace(content, match => {
                        var url = match.Value;
                        return $"<a href=\"{url}\">{url}</a>";
                    });
                    return html;
                }

                var encodedContent = HttpUtility.HtmlEncode(Content);
                return LinkUrls(encodedContent);
            }
        }
    }
}
