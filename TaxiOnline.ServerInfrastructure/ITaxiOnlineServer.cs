using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;
using TaxiOnline.ServerInfrastructure.LogicInterfaces;

namespace TaxiOnline.ServerInfrastructure
{
    public interface ITaxiOnlineServer
    {
        IEnumerable<ICityLogic> Cities { get; }
        void LoadPersistentState();
        ICityInfo CreateCityInfo(Guid guid);
        IEnumerable<ICityInfo> EnumerateCities(string userCultureName);
        IPedestrianInfo CreatePedestrianInfo();
        IPedestrianInfo CreatePedestrianInfo(Guid id);
        IDriverInfo CreateDriverInfo();
        IDriverInfo CreateDriverInfo(Guid id);
        void AuthenticateAsPedestrian(IPedestrianInfo pedestrianInfo, Guid cityId);
        void AuthenticateAsDriver(IDriverInfo driverInfo, Guid cityId);
        IPedestrianRequestInfo CreatePedestrianRequestInfo(IPedestrianInfo pedestrian, IDriverInfo driver);
        IPedestrianRequestInfo CreatePedestrianRequestInfo(Guid id, Guid pedestrianId, Guid driverId);
        void PushPedestrianRequestInfo(IPedestrianRequestInfo requestInfo);
    }
}
