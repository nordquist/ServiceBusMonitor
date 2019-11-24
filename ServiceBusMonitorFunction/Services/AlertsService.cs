using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceBusMonitorFunction.Models;
using ServiceBusMonitorFunction.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Alert = ServiceBusMonitorFunction.Models.Alert;

namespace ServiceBusMonitorFunction.Services
{
    public class AlertsService : IAlertsService
    {
        private readonly IConfiguration _configuration;
        private readonly IMailQueueClient _mailQueueClient;
        public ILogger Logger { get; set; }

        public AlertsService(IConfiguration configuration, IMailQueueClient mailQueueClient)
        {
            _configuration = configuration;
            _mailQueueClient = mailQueueClient;
        }

        public async Task<Alert> AuditAsync(ElasticQueueDetails existingDetails, ElasticQueueDetails createdDetails)
        {
            if (createdDetails.DeadletterMessagesCount < 1 && existingDetails.DeadletterMessagesCount < 1)
                return null;

            var lowThresholdStr = _configuration["AlertRules:LowThreshold"];
            var mediumThresholdStr = _configuration["AlertRules:MediumThreshold"];
            var highThresholdStr = _configuration["AlertRules:HighThreshold"];
            var differentiatorThresholdStr = _configuration["AlertRules:FiveMinutePercentageThreshold"];

            if (!long.TryParse(lowThresholdStr, out var lowThreshold))
            {
                Logger.LogWarning($"Could not parse lowThreshold '{lowThresholdStr}', setting to default 5");
                lowThreshold = 5;
            }

            if (!long.TryParse(mediumThresholdStr, out var mediumThreshold))
            {
                Logger.LogWarning($"Could not parse mediumThreshold '{mediumThresholdStr}', setting to default 15");
                mediumThreshold = 15;
            }

            if (!long.TryParse(highThresholdStr, out var highThreshold))
            {
                Logger.LogWarning($"Could not parse highThreshold '{highThresholdStr}', setting to default 100");
                highThreshold = 100;
            }

            if (!long.TryParse(differentiatorThresholdStr, out var differentiatorThreshold))
            {
                Logger.LogWarning($"Could not parse differentiatorThreshold '{differentiatorThresholdStr}', setting to default 50");
                differentiatorThreshold = 50;
            }

            var difference = createdDetails.DeadletterMessagesCount - existingDetails.DeadletterMessagesCount;
            var increase = difference / existingDetails.DeadletterMessagesCount * 100;

            if (increase < 0)
            {
                Logger.LogInformation($"Since the deadletter count is decreasing, no more alerts will be generated.");
                return null;
            }

            if (increase > differentiatorThreshold)
            {
                var message = $"Deadletter message count on '{createdDetails.QueueName}' is {createdDetails.DeadletterMessagesCount} and has increased with {increase}% since last run.";
                return new Alert(Severity.High, "High increase on deadletter", message);
            }

            if (createdDetails.DeadletterMessagesCount > highThreshold)
                return new Alert(Severity.High, "High deadletter count", $"Deadletter message count on '{createdDetails.QueueName}' is {createdDetails.DeadletterMessagesCount}");

            if (createdDetails.DeadletterMessagesCount > mediumThreshold)
                return new Alert(Severity.Medium, "Medium deadletter count", $"Deadletter message count on '{createdDetails.QueueName}' is {createdDetails.DeadletterMessagesCount}");

            if (createdDetails.DeadletterMessagesCount > lowThreshold)
                return new Alert(Severity.Low, "Notable deadletter count", $"Deadletter message count on '{createdDetails.QueueName}' is {createdDetails.DeadletterMessagesCount}");

            return null;
        }

        public async Task<List<string>> ProcessAlertsAsync(List<Alert> alerts)
        {
            var correlationIds = new List<string>();
            foreach (var alert in alerts)
            {
                var templateStr = _configuration["PostMarkWarningTemplateId"];

                if (string.IsNullOrWhiteSpace(templateStr))
                {
                    Logger.LogWarning($"No PostMark template available in settings, cannot proceed");
                    return correlationIds;
                }

                if (!long.TryParse(templateStr, out var templateId))
                {
                    Logger.LogWarning($"Erroneous PostMark template Id {templateStr}, cannot parse to long");
                    return correlationIds;
                }

                var email = new Email
                {
                     From = _configuration["EmailConfig:Sender"],
                     TemplateId = templateId,
                     TemplateModel = new Dictionary<string, string>
                     { 
                         { "title", $"Happy at Work - {alert.Header} - [internal]" },
                         { "header", $"{alert.Header}" },
                         { "message", $"{alert.Message}" }
                     },
                     To = _configuration["EmailConfig:Recipient"]
                };

                var correlationId = await _mailQueueClient.QueueGenericEmailAsync(email, Logger);
                correlationIds.Add(correlationId);
            }

            return correlationIds;
        }
    }
}
