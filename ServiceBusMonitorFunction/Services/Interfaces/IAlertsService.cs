using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceBusMonitorFunction.Models;
using Microsoft.Extensions.Logging;

namespace ServiceBusMonitorFunction.Services.Interfaces
{
    public interface IAlertsService
    {
        Task<Alert> AuditAsync(ElasticQueueDetails existingDetails, ElasticQueueDetails createdDetails);
        Task<List<string>> ProcessAlertsAsync(List<Alert> alerts);
        ILogger Logger { get; set; }
    }
}