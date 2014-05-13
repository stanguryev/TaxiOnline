using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.ServerInfrastructure
{
    public interface ITaxiOnlineStorage
    {
        IEnumerable<ICityInfo> EnumerateCities();
        IEnumerable<ICityInfo> EnumerateCities(string userCultureName);
        IEnumerable<IPedestrianInfo> EnumeratePedestrians(Guid cityId);
        IEnumerable<IDriverInfo> EnumerateDrivers(Guid cityId);
        IEnumerable<IPedestrianRequestInfo> EnumeratePedestrianRequests(Guid cityId);
        IEnumerable<IDriverResponseInfo> EnumerateDriverResponses(Guid cityId);
        ActionResult AddDriver(IDriverInfo driverInfo);
        ActionResult AddPedestrian(IPedestrianInfo pedestrianInfo);
        ActionResult RemoveDriver(IDriverInfo driverInfo);
        ActionResult RemovePedestrian(IPedestrianInfo pedestrianInfo);
    }
}
