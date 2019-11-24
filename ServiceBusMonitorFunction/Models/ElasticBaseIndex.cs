using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceBusMonitorFunction.Models
{
    public abstract class ElasticBaseIndex
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public DateTime Updated { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(5)]
        public string Environment { get; set; }

        public string Comment { get; set; }
    }
}