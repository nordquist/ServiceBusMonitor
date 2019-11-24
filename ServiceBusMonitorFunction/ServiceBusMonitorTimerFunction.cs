using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceBusMonitorFunction;
using ServiceBusMonitorFunction.Models;
using ServiceBusMonitorFunction.Services.Interfaces;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Logging;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup))]
namespace ServiceBusMonitorFunction
{
    public static class ServiceBusMonitorTimerFunction
    {
        [FunctionName("ServiceBusMonitorTimerFunction")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo timer, ILogger log, [Inject] IElasticService elasticService, [Inject] IAlertsService alertsService)
        {
            log.LogInformation($"{nameof(ServiceBusMonitorTimerFunction)} function executed at: {DateTime.UtcNow:yyyy-MM-dd HH::mm:ss}");

            try
            {
                alertsService.Logger = log;
                var elasticIndex = Environment.GetEnvironmentVariable("ElasticIndex");
                var managementClient = new ManagementClient(Environment.GetEnvironmentVariable("ServiceBusConnection"));
                var queues = await managementClient.GetQueuesAsync();
            
                var alertList = new List<Alert>();

                foreach (var queueDescription in queues)
                {
                    log.LogInformation($"Investigating queue '{queueDescription.Path}'");
                    var queue = await managementClient.GetQueueRuntimeInfoAsync(queueDescription.Path);
                    var activeMessageCount = queue.MessageCountDetails.ActiveMessageCount;
                    var deadLetterMessageCount = queue.MessageCountDetails.DeadLetterMessageCount;
                    log.LogInformation($"Active messages: {activeMessageCount}");
                    log.LogInformation($"Dead letter messages: {deadLetterMessageCount}");

                    var list = await elasticService.ElasticQueueDetailsRepository.GetListAsync(queueDescription.Path);
                    var single = list.FirstOrDefault();

                    if (single == null)
                    {
                        var queueDetails = new ElasticQueueDetails(queueDescription.Path)
                        {
                            ActiveMessagesCount = activeMessageCount,
                            DeadletterMessagesCount = deadLetterMessageCount,
                            TransferMessagesCount = queue.MessageCountDetails.TransferMessageCount
                        };
                        await elasticService.PostAsync(elasticIndex, queueDetails);
                    }
                    else
                    {
                        var alert = await alertsService.AuditAsync(single, new ElasticQueueDetails(queueDescription.Path)
                        {
                            ActiveMessagesCount = activeMessageCount,
                            DeadletterMessagesCount = deadLetterMessageCount,
                            TransferMessagesCount = queue.MessageCountDetails.TransferMessageCount
                        });

                        if (alert != null)
                        {
                            log.LogInformation($"Found alert and added for dispatch," +
                              $"{Environment.NewLine}" +
                              $"{Enum.GetName(typeof(Severity), alert.Severity)}" +
                              $"{Environment.NewLine}" +
                              $"Details: '{alert.Message}', {queueDescription.Path}" +
                              $"{Environment.NewLine}");

                            alertList.Add(alert);
                        }

                        single.ActiveMessagesCount = activeMessageCount;
                        single.DeadletterMessagesCount = deadLetterMessageCount;
                        single.TransferMessagesCount = queue.MessageCountDetails.TransferMessageCount;
                        single.Updated = DateTime.UtcNow;
                        await elasticService.PutAsync(elasticIndex, single);
                    }
                }

                if (alertList.Count > 0)
                {
                    var correlations = await alertsService.ProcessAlertsAsync(alertList);
                    foreach (var correlationId in correlations)
                        log.LogInformation($"Mail message '{correlationId}' was added to queue");
                }
            }
            catch (Exception exception)
            {
                log.LogError($"Error in {nameof(ServiceBusMonitorTimerFunction)}, details: {exception.Message}");
                throw;
            }
        }
    }
}
