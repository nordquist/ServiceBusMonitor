using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceBusMonitorFunction.Models;
using ServiceBusMonitorFunction.Services.Interfaces;

namespace ServiceBusMonitorFunction.Services
{
    public class MailQueueClient : IMailQueueClient
    {
        public async Task<string> QueueGenericEmailAsync(Email mail, ILogger log)
        {
            var objectId = Guid.NewGuid();
            var correlationId = $"{mail.From}_{mail.To}_{objectId}_{DateTime.UtcNow}";
            log.LogInformation($"Queueing e-mail to '{mail.To}' with Id '{objectId}'");

            var connectionStringKey = Environment.GetEnvironmentVariable("ServiceBusConnection");
            var queueName = Environment.GetEnvironmentVariable("MailQueue");
            var client = new QueueClient(connectionStringKey, queueName);

            var json = JsonConvert.SerializeObject(mail);
            var messageContent = Encoding.UTF8.GetBytes(json);
            var message = CreateBaseMessage(messageContent, correlationId);
            var success = false;
            var retries = 0;

            while (!success || retries < 3)
            {
                try
                {
                    await client.SendAsync(message);
                    success = true;
                    break;
                }
                catch (Exception exception)
                {
                    log.LogWarning($"Could not add e-mail '{message.CorrelationId}' to queue, details: '{exception.Message}'");
                    retries++;
                }

                log.LogWarning($"Could not add e-mail '{message.CorrelationId}' to queue");
            }

            return correlationId;
        }

        private static Message CreateBaseMessage(byte[] byteContent, string correlationId)
        {
            var message = new Message(byteContent)
            {
                ContentType = "application/json",
                CorrelationId = correlationId
            };
            return message;
        }
    }
}