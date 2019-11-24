using System.Collections.Generic;

namespace ServiceBusMonitorFunction.Models
{
    public class Email
    {
        public string To { get; set; }
        public string From { get; set; }
        public long TemplateId { get; set; }
        public Dictionary<string, string> TemplateModel { get; set; }
    }
}