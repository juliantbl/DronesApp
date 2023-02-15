using Drones.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Drones.Services
{
    public class ValidationService
    {
        public List<Delivery> FilterValidDeliveries(List<Delivery> deliveries, decimal maxCapacity) 
        {
            var totalDeliveries = deliveries.Count();
            var validDeliveries= deliveries.Where(t => t.Weight < maxCapacity).ToList();
            var noPossibleDeliveries = totalDeliveries- validDeliveries.Count();

            if (noPossibleDeliveries > 0)
             Console.WriteLine(noPossibleDeliveries + " packages will not be delivered due to exceeding the drones max capacity");
            
            return validDeliveries;
        }

    }
}
