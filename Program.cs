using System;
using System.Linq;
namespace Drones
{
    using Drones.Types;
    using System;
    using System.Collections.Generic;

    namespace BoxPacking
    {
        class Program
        {
            static void Main(string[] args)
            {
                #region Input Data

                List<Drone> drones = new List<Drone>();
                while (!drones.Any())
                {
                    Console.WriteLine("Enter the Drones names and capacities separated by , :");
                    string dronesString = Console.ReadLine();
                    drones = ParseDrones(dronesString);
                    if (!drones.Any()) Console.WriteLine("No drone data was detected, please retry");
                }

                string locationString = "";
                List<Delivery> trips = new List<Delivery>();

                while (!trips.Any())
                {
                    Console.WriteLine("Enter the Locations and deliver weight separated by ,");
                    
                    bool continueTyping = true;
                    while (continueTyping)
                    {
                        string newTrip = Console.ReadLine();
                        Console.WriteLine("Press Enter to insert more locations or Esc to continue");
                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        locationString += newTrip+",";
                        if (keyInfo.Key == ConsoleKey.Escape) break;
                    }
                    locationString = locationString.Replace("\r", string.Empty);
                    trips = ParseTrips(locationString);
                    if (!trips.Any()) Console.WriteLine("No location data was detected, please retry");
                }

                #endregion
                //For low the processing cost the drones and trips will be order and process in descending order
                #region Data filters

                drones = drones.OrderByDescending(x => x.MaxCapacity).ToList();
                trips = trips.OrderByDescending(x => x.Weight).ToList();

                var noPossibleTrips = trips.Count(t=>t.Weight> drones.First().MaxCapacity);

                if (noPossibleTrips>0)
                {
                    Console.WriteLine("The following trips will be skipped because there are no available drones to carry that amount of weight: ");
                    for (int i = 0; i < noPossibleTrips; i++) 
                        Console.WriteLine(trips[i].Location+"," +trips[i].Weight);
                    Console.WriteLine();
                }

                var posibleTrips = trips.Skip(noPossibleTrips).ToList();
                #endregion

                #region processing
                var generalMaxCapacity = drones.FirstOrDefault().MaxCapacity;

                while (posibleTrips.Any())
                {
                    var newDelivery = PackDelivery(posibleTrips, generalMaxCapacity);
                    PackByCompleteness(drones,newDelivery);
                }
                #endregion

                #region presentation

                drones = drones.OrderBy(d => d.Name).ToList();

                foreach (var drone in drones)
                {
                    Console.WriteLine("Trips for  "+ drone.Name+ ":");
                    var numberOfTrips = drone.Trips?.Count;
                    if (numberOfTrips>0)
                    {
                        for (int i = 0; i < drone.Trips.Count; i++)
                        {
                            Console.WriteLine("Trip #"+(i+1));

                            foreach (var delivery in drone.Trips[i])
                            {
                                Console.WriteLine(delivery.Location+","+delivery.Weight);
                            }
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("No trips assigned");
                    }
                }
                Console.ReadLine();
                #endregion
            }

            private static List<Drone> ParseDrones(string drones)
            {
                List<Drone> result = new List<Drone>();
                List<string> dronesProps = drones.Replace(" ", string.Empty).Split(",").ToList();
                for (int i = 0; i < dronesProps.Count; i += 2)
                {
                    result.Add(new Drone
                    {
                        Name = dronesProps[i],
                        MaxCapacity = int.Parse(dronesProps[i + 1].Trim('[', ']'))
                    });
                }
                return result;
            }

            private static List<Delivery> ParseTrips(string trips)
            {
                List<Delivery> result = new List<Delivery>();
                List<string> tripsProps = trips.Replace(" ",string.Empty).Split(",").ToList();
                tripsProps = tripsProps.Where(p => p != string.Empty).ToList();
                for (int i = 0; i < tripsProps.Count; i += 2)
                {
                    result.Add(new Delivery
                    {
                        Location = tripsProps[i],
                        Weight = int.Parse(tripsProps[i + 1].Trim('[', ']'))
                    });
                }
                return result;
            }

            static List<Delivery> PackDelivery( List<Delivery> deliveries, int capacity)
            {
                var tripsPacked = new List<Delivery>();
                //The algotithm will distribute the trips based on the max capacity provided by drones
                var currentCap = capacity;
                var heavierPack = deliveries.FirstOrDefault();
                currentCap -= heavierPack.Weight;
                tripsPacked.Add(heavierPack);
                deliveries.Remove(heavierPack);
                if (currentCap==0) return tripsPacked;

                var weights = deliveries.Select(d=>d.Weight).ToList();

                //Find the closest distribution to fill all the capacity
                var weightsToPack= FindMaxSumElements(weights, currentCap);

                foreach (var weight in weightsToPack)
                {
                    var newDelivery = deliveries.FirstOrDefault(d=>d.Weight== weight);
                    tripsPacked.Add(newDelivery);
                    deliveries.Remove(newDelivery);
                }
                return tripsPacked;
            }

            static void PackByCompleteness(List<Drone> drones, List<Delivery> trip) 
            {
                var tripWeight = trip.Sum(x => x.Weight);
                var completeness = 0;
                Drone maxCompletenessDrone = null;
                foreach (var drone in drones)
                {
                    var droneCompleteness = (100 * tripWeight) / drone.MaxCapacity;
                    if (droneCompleteness<=100 && completeness< droneCompleteness)
                    {
                        maxCompletenessDrone = drone;
                    }
                }
                if (maxCompletenessDrone.Trips==null)  maxCompletenessDrone.Trips = new List<List<Delivery>>();
                
                maxCompletenessDrone.Trips.Add(trip);
            }

            static List<int> FindMaxSumElements(List<int> nums, int goal)
            {
                var n = nums.Count;
                var sums = new int[n + 1, goal + 1];
                var result = new List<int>();

                for (int i = 0; i <= n; i++)
                {
                    //For each element in nums
                    for (int j = 0; j <= goal; j++)
                    {
                        var numIdx = i == 0 ? 0: i - 1;
                        var num = nums[numIdx];
                        var numberDiagUp = j > num ? sums[numIdx, j - num]:0;
                        var numberUp = sums[numIdx, j];

                        //Initial cases
                        if (i == 0 || j == 0) sums[i, j] = 0;                        
                        else if (num <= j  && numberDiagUp + num > numberUp)
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
                    if (sums[n, goal] == sums[n - 1, goal]) n--;
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
}
