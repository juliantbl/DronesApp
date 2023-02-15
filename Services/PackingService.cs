using Drones.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drones.Services
{
    public class PackingService
    {
        public List<DeliveryRoute> DistributePackages(List<Delivery> deliveries, decimal dronesMaxCapacity, List<Drone> drones)
        {
            var distribution = new List<DeliveryRoute>();

            while (deliveries.Any())
            {
                var deliveryRoute = PackDelivery(deliveries, dronesMaxCapacity);

                var drone = FindRouteBestDrone(drones, deliveryRoute.Route);

                deliveryRoute.AssignedDrone = drone;

                distribution.Add(deliveryRoute);
            }
            return distribution;
        }
        public DeliveryRoute PackDelivery(List<Delivery> deliveries, decimal capacity)
        {
            //The algotithm will distribute the routes based on the max capacity provided by drones
            var deliveryRoute= new DeliveryRoute(null, new List<Delivery>());

            var currentCapacity = capacity;

            var heavierPack = deliveries.OrderByDescending(x=>x.Weight).FirstOrDefault();

            currentCapacity -= heavierPack.Weight;

            deliveryRoute.Route.Add(heavierPack);

            deliveries.Remove(heavierPack);

            if (currentCapacity == 0) return deliveryRoute;

            PackByDistribution(deliveries, currentCapacity, deliveryRoute);

            return deliveryRoute;
        }

        public Drone FindRouteBestDrone(List<Drone> drones, List<Delivery> route)
        {
            var routeTotalWeight = route.Sum(x => x.Weight);

            var completeness = 0;

            Drone maxCompletenessDrone = null;

            foreach (var drone in drones)
            {
                var droneCompleteness = (100 * routeTotalWeight) / drone.MaxCapacity;

                if (droneCompleteness <= 100 && completeness < droneCompleteness)  maxCompletenessDrone = drone;               
            }

           return maxCompletenessDrone;
        }

        static void PackByDistribution(List<Delivery> deliveries, decimal currentCapacity, DeliveryRoute deliveryRoute)
        {
            //Find the closest distribution to fill all the capacity

            var weightsToCarry = deliveries.Select(d => d.Weight).ToList();

            var weightsToPack = FindMaxSumElements(weightsToCarry, currentCapacity);

            foreach (var weight in weightsToPack)
            {
                var route = deliveries.FirstOrDefault(d => d.Weight == weight);
                deliveryRoute.Route.Add(route);
                deliveries.Remove(route);
            }
        }


        static List<decimal> FindMaxSumElements(List<decimal> nums, decimal goal)
        {
            var n = nums.Count;
            var goalInt= (int)Math.Ceiling(goal);
            var sums = new decimal[n + 1, goalInt + 1];
            var result = new List<decimal>();

            for (int i = 0; i <= n; i++)
            {
                //For each element in nums
                for (int j = 0; j <= goal; j++)
                {
                    var numIdx = i == 0 ? 0 : i - 1;
                    var num = nums[numIdx];
                    var numberDiagUp = j > num ? sums[numIdx, j - (int)Math.Ceiling(num)] : 0;
                    var numberUp = sums[numIdx, j];

                    //Initial cases
                    if (i == 0 || j == 0) sums[i, j] = 0;
                    else if (num <= j && numberDiagUp + num > numberUp)
                    {
                        sums[numIdx + 1, j] = numberDiagUp + num;
                    }
                    else
                        sums[numIdx + 1, j] = numberUp;
                }
            }
            //Backtracking to get the original distribution
            while (n > 0 && goal > 0)
            {
                if (sums[n, goalInt] == sums[n - 1, goalInt]) n--;
                else
                {
                    result.Add(nums[n - 1]);
                    goal -= nums[n - 1];
                    n--;
                }
            }
            return result;
        }
    }
}
