using System;
using System.Collections.Generic;
using System.Text;

namespace Drones.Types
{
    public class Drone
    {
        public string Name { get; set; }
        public int MaxCapacity { get; set; }
        public List<List<Delivery>> Trips { get; set; }
    }
}
