using System;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using ServiceBusMonitorFunction.Models;
using ServiceBusMonitorFunction.Repositories;
using ServiceBusMonitorFunction.Services.Interfaces;

namespace ServiceBusMonitorFunction.Services
{
    public class ElasticService : IElasticService
    {
        private static ElasticClient _elasticClient;
        private ElasticQueueDetailsRepository _elasticQueueDetailsRepository;

        public ElasticQueueDetailsRepository ElasticQueueDetailsRepository => _elasticQueueDetailsRepository ??= new ElasticQueueDetailsRepository(_elasticClient);

        public ElasticService(string connectionString)
        {
            var node = new Uri(connectionString);
            var settings = new ConnectionSettings(node);

            settings.DisableDirectStreaming();
            _elasticClient = new ElasticClient(settings);
        }

        public async Task<IndexResponse> PostAsync<T>(string index, T model) where T : ElasticBaseIndex
        {
            var result = await _elasticClient.SearchAsync<T>(x => x.Index(index).Size(1).
                Sort(sort => sort.Descending(f => f.Id)).
                Query(q => q.MatchAll()));

            var highest = result.Documents.OrderByDescending(d => d.Id).Select(d => d.Id).FirstOrDefault();

            model.Id = highest + 1;
            model.Created = DateTime.Now;

            var postResponse = await _elasticClient.IndexAsync(model, i => i.Index(index).Id(model.Id).Refresh(Refresh.True));
            return postResponse;
        }

        public async Task<IndexResponse> PutAsync<T>(string index, T model) where T : ElasticBaseIndex
        {
            var postResponse = await _elasticClient.IndexAsync(model, i => i.Index(index).Refresh(Refresh.True));
            return postResponse;
        }
    }
}
