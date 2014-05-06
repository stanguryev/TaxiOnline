using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;

namespace TaxiOnline.ServerInfrastructure.LogicInterfaces
{
    public interface ICityLogic
    {
        ICityInfo Info { get; }
        IEnumerable<IPedestrianInfo> Pedestrians { get; }
        IEnumerable<IDriverInfo> Drivers { get; }
        IEnumerable<IPedestrianRequestsInfo> PedestrianRequests { get; }
        void ModifyPedestriansCollection(Action<IList<IPedestrianInfo>> modificationDelegate);
        void ModifyDriversCollection(Action<IList<IDriverInfo>> modificationDelegate);
        void ModifyPedestrianRequestsCollection(Action<IList<IPedestrianRequestsInfo>> modificationDelegate);
    }
}
