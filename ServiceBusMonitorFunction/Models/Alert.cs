namespace ServiceBusMonitorFunction.Models
{
    public enum Severity { Low, Medium, High }

    public class Alert
    {
        public Severity Severity { get; }
        public string Header { get; }
        public string Message { get; }

        public Alert(Severity severity, string header, string message)
        {
            Severity = severity;
            Header = header;
            Message = message;
        }
    }
}
