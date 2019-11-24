using System.Threading.Tasks;
using Nest;
using ServiceBusMonitorFunction.Models;

namespace ServiceBusMonitorFunction.Services.Interfaces
{
    public interface IElasticService
    {
        IElasticQueueDetailsRepository ElasticQueueDetailsRepository { get; }
        Task<IndexResponse> PostAsync<T>(string index, T model) where T : ElasticBaseIndex;
        Task<IndexResponse> PutAsync<T>(string index, T model) where T : ElasticBaseIndex;
    }
}
