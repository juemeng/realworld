using System;
using System.Collections.Generic;

namespace Moniter.Models
{
    public class Room:Entity
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public IEnumerable<Bed> Beds { get; set; }
        
        public Guid FloorId { get; set; }
        public Floor Floor { get; set; }
    }
}