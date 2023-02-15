using Drones.Domain.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Drones.Services
{
    public class InputService
    {
        public List<string> RequestInput()
        {
            List<string> input = null;
            while (input == null)
            {
                Console.WriteLine("Enter the file path to the input data: ");
                string filePath = Console.ReadLine();
                input = ReadFile(filePath)?.ToList();
            }
            return input;
        }
        public string[] ReadFile(string filePath)
        {

            if (!File.Exists(filePath))
            {
                Console.WriteLine("The file does not exist.");
                return null;
            }

            string[] lines = File.ReadAllLines(filePath);

            lines = Array.FindAll(lines, l => !string.IsNullOrEmpty(l));

            return lines;
        }

        public  List<Drone> ParseDrones(string drones)
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

        public  List<Delivery> ParseDeliveries(List<string> textDeliveries)
        {
            List<Delivery> result = new List<Delivery>();

            foreach (var txtDelivery in textDeliveries)
            {
                var props = txtDelivery.Replace(" ", string.Empty).Split(",");
                result.Add(new Delivery
                {
                    Location = props[0],
                    Weight = int.Parse(props[1].Trim('[', ']'))
                });
            }
            return result;
        }

        public void PresentDroneRoutes(List<Drone> drones,List<DeliveryRoute> routes )
        {
            drones = drones.OrderBy(d => d.Name).ToList();

            foreach (var drone in drones)
            {
                Console.WriteLine("Trips for  " + drone.Name + ":");

                List<DeliveryRoute> droneRoutes = routes.Where(d=>d.AssignedDrone?.Name== drone.Name).ToList();

                var numberOfRoutes = droneRoutes?.Count();

                if (numberOfRoutes > 0)
                {
                    for (int i = 0; i < numberOfRoutes; i++)
                    {
                        Console.WriteLine("Trip #" + (i + 1));

                        foreach (var delivery in droneRoutes[i].Route)
                        {
                            Console.WriteLine(delivery.Location + "," + delivery.Weight);
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
        }
    }
}
