using System;
using System.Collections.Generic;
using System.Text;

namespace Drones.Domain.Entity
{
    public class DeliveryRoute
    {
        public DeliveryRoute(Drone assignedDrone, List<Delivery> route)
        {
            AssignedDrone = assignedDrone;
            Route = route;
        }

        public Drone AssignedDrone { get; set; }
        public List<Delivery> Route { get; set; }
    }
}
