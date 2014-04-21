using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;

namespace TaxiOnline.ServerInfrastructure
{
    public interface ITaxiOnlineServer
    {
        IEnumerable<IPedestrianInfo> Pedestrians { get; }
        IEnumerable<IDriverInfo> Drivers { get; }
        void LoadPersistentState();
        ICityInfo CreateCityInfo(Guid guid);
        IEnumerable<ICityInfo> EnumerateCities(string userCultureName);
        IPedestrianInfo CreatePedestrianInfo();
        IPedestrianInfo CreatePedestrianInfo(Guid id);
        IDriverInfo CreateDriverInfo();
        void ModifyPedestriansCollection(Action<IList<IPedestrianInfo>> modificationDelegate);
        void ModifyDriversCollection(Action<IList<IDriverInfo>> modificationDelegate);

    }
}
