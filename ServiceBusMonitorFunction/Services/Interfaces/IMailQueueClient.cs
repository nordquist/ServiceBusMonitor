using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiceBusMonitorFunction.Models;

namespace ServiceBusMonitorFunction.Services.Interfaces
{
    public interface IMailQueueClient
    {
        Task<string> QueueGenericEmailAsync(Email mail, ILogger log);
    }
}