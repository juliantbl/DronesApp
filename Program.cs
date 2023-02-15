using System.Linq;
namespace Drones
{
    using Drones.Services;

    namespace BoxPacking
    {
        class Program
        {
            static void Main(string[] args)
            {
                #region Services
                var inputService = new InputService();

                var validationService = new ValidationService();

                var packingService= new PackingService();
                #endregion

                #region Input Data
                var input = inputService.RequestInput();

                var drones = inputService.ParseDrones(input.FirstOrDefault());

                var deliveries = inputService.ParseDeliveries(input.Skip(1).ToList());

                #endregion

                #region Data filters

                drones = drones.OrderByDescending(x => x.MaxCapacity).ToList();
               
                var dronesMaxCapacity = drones.First().MaxCapacity;

                deliveries = deliveries.OrderByDescending(x => x.Weight).ToList();

                deliveries = validationService.FilterValidDeliveries(deliveries, dronesMaxCapacity);
                #endregion

                #region Processing

                var distribution = packingService.DistributePackages(deliveries, dronesMaxCapacity, drones);

                #endregion

                #region presentation
                inputService.PresentDroneRoutes(drones, distribution);
                #endregion
            }
        }  
    }
}
