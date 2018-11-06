using System;

namespace Moniter.Features.Deploy
{
    public class DeployInfo
    {
        public string Host { get; set; }
        public int Port { get; set; } = 22;
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid FloorId { get; set; }
        public Guid MasterId { get; set; }
        public string ServerAlias { get; set; }
    }
}