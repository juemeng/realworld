using System;

namespace Moniter.Models
{
    public class Binding : Entity
    {
        public string Host { get; set; }
        public int Port { get; set; } = 22;
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid FloorId { get; set; }
        public Floor Floor { get; set; }
    }
}