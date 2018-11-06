using System;
using Moniter.Models;

namespace Moniter.Features.Parser
{
    public class AlertInfo
    {
        public AlertMessage Message { get; set; }
        public string Description { get; set; }
        public Guid MasterId { get; set; }
        public string SlaveNumber { get; set; }
        public DateTime AlertTime { get; set; }
    }
}