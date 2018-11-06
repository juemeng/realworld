using System;
using System.Collections.Generic;

namespace Moniter.Models
{
    public class Floor:Entity
    {
        public string Description { get; set; }
        public int Number { get; set; }
        public IEnumerable<Room> Rooms { get; set; }
        public Guid? MasterId { get; set; }
        public Guid BuildingId { get; set; }
        public Building Building { get; set; }
        public Binding Binding { get; set; }
    }
}