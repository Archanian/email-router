using System.Collections.Generic;

namespace EmailRouter.Domain.Emails
{
    public class Email
    {
        public string From { get; set; }
        public string To { get; set; }
        public IEnumerable<string> Cc { get; set; }
        public IEnumerable<string> Bcc { get; set; }
        public string Subject { get; set; }
        public string Tag { get; set; }
        public string HtmlBody { get; set; }
        public string TextBody { get; set; }
        public string ReplyTo { get; set; }
        public IEnumerable<Header> Headers { get; set; }
        public bool TrackOpens { get; set; }
        public TrackLinks TrackLinks { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }
    }
}