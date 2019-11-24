using System;

namespace ServiceBusMonitorFunction.Models
{
    public abstract class ElasticBaseIndex
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string Environment { get; set; }

        public string Comment { get; set; }
    }
}