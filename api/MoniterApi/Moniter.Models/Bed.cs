using System;

namespace Moniter.Models
{
    public class Bed:Entity
    {
        public int Number { get; set; }
        public string SlaveNumber { get; set; }
        public Guid RoomId { get; set; }
        public Room Room { get; set; }
    }
}