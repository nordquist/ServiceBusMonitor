using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceBusMonitorFunction.Models;

namespace ServiceBusMonitorFunction.Services.Interfaces
{
    public interface IElasticQueueDetailsRepository
    {
        Task<List<ElasticQueueDetails>> GetListAsync(string queueName);
    }
}