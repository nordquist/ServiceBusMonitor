namespace ServiceBusMonitorFunction.Models
{
    public class ElasticQueueDetails : ElasticBaseIndex
    {
        public string QueueName { get; }
        public long ActiveMessagesCount { get; set; }
        public long DeadletterMessagesCount { get; set; }
        public long TransferMessagesCount { get; set; }

        public ElasticQueueDetails(string queueName)
        {
            Comment = $"{queueName}";
            QueueName = queueName;
            Environment = System.Environment.GetEnvironmentVariable("Environment");
        }

        public ElasticQueueDetails(ElasticQueueDetails details)
        {
            Comment = $"{details.QueueName}";
            QueueName = details.QueueName;
            Environment = System.Environment.GetEnvironmentVariable("Environment");
        }
    }
}