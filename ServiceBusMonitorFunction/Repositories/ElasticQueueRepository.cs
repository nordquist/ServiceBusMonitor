using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using ServiceBusMonitorFunction.Models;
using ServiceBusMonitorFunction.Services.Interfaces;

namespace ServiceBusMonitorFunction.Repositories
{
    public class ElasticQueueDetailsRepository : IElasticQueueDetailsRepository
    {
        private static ElasticClient _elasticClient;
        private const string Index = "happyatwork-queue";

        public ElasticQueueDetailsRepository(ElasticClient client)
        {
            _elasticClient = client;
        }

        public async Task<List<ElasticQueueDetails>> GetListAsync(string queueName)
        {
            var result = await _elasticClient.SearchAsync<ElasticQueueDetails>(c => c.Index(Index).Query(q => q
                .MatchPhrasePrefix(m => m.Field("queueName").Query(queueName))));

            var list = result.Documents.Select(d => d).ToList();
            return list;
        }
    }
}
