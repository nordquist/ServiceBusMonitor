using System.Threading.Tasks;
using Nest;
using ServiceBusMonitorFunction.Models;
using ServiceBusMonitorFunction.Repositories;

namespace ServiceBusMonitorFunction.Services.Interfaces
{
    public interface IElasticService
    {
        ElasticQueueDetailsRepository ElasticQueueDetailsRepository { get; }
        Task<IndexResponse> PostAsync<T>(string index, T model) where T : ElasticBaseIndex;
        Task<IndexResponse> PutAsync<T>(string index, T model) where T : ElasticBaseIndex;
    }
}
