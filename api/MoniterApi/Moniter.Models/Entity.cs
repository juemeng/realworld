using System;

namespace Moniter.Models
{
    public class Entity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedTime { get; set; } = DateTime.Now;
    }
}