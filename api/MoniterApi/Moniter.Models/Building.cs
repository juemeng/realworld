using System;
using System.Collections.Generic;

namespace Moniter.Models
{
    public class Building:Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<Floor> Floors { get; set; }
    }
}