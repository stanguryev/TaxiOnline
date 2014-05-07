using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.Server.Core.Objects;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;
using TaxiOnline.ServerInfrastructure.LogicInterfaces;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.CollectionsDecorators;

namespace TaxiOnline.Server.Core.Logic
{
    internal class CityLogic : ICityLogic
    {
        private readonly LogicExtender _extender;
        private readonly CityInfo _info;
        private readonly ReadonlyCollectionDecorator<PedestrianInfo> _pedestrians = new ReadonlyCollectionDecorator<PedestrianInfo>();
        private readonly ReadonlyCollectionDecorator<DriverInfo> _drivers = new ReadonlyCollectionDecorator<DriverInfo>();
        private readonly ReadonlyCollectionDecorator<PedestrianRequestsInfo> _pedestrianRequests = new ReadonlyCollectionDecorator<PedestrianRequestsInfo>();
        private ReadonlyCollectionDecorator<DriverResponseInfo> _driverResponses = new ReadonlyCollectionDecorator<DriverResponseInfo>();

        public ICityInfo Info
        {
            get { return _info; }
        }

        public IEnumerable<IPedestrianInfo> Pedestrians
        {
            get { return _pedestrians.Items; }
        }

        public IEnumerable<IDriverInfo> Drivers
        {
            get { return _drivers.Items; }
        }

        public IEnumerable<IPedestrianRequestsInfo> PedestrianRequests
        {
            get { return _pedestrianRequests.Items; }
        }

        public IEnumerable<IDriverResponseInfo> DriverResponses
        {
            get { return _driverResponses.Items; }
        }

        public CityLogic(CityInfo info, TaxiOnlineServer server, LogicExtender extender)
        {
            _info = info;
            _extender = extender;
        }

        public void LoadPersistentState()
        {
            List<IPedestrianInfo> pedestrians = _extender.Storage.EnumeratePedestrians(_info.Id).ToList();
            List<IDriverInfo> drivers = _extender.Storage.EnumerateDrivers(_info.Id).ToList();
            _pedestrians.ModifyCollection<IPedestrianInfo>(col => pedestrians.ForEach(p => col.Add(p)));
            _drivers.ModifyCollection<IDriverInfo>(col => drivers.ForEach(d => col.Add(d)));
        }

        [Obsolete]
        public void ModifyPedestriansCollection(Action<IList<IPedestrianInfo>> modificationDelegate)
        {
            _pedestrians.ModifyCollection(modificationDelegate);
        }

        [Obsolete]
        public void ModifyDriversCollection(Action<IList<IDriverInfo>> modificationDelegate)
        {
            _drivers.ModifyCollection(modificationDelegate);
        }

        public void ModifyPedestrianRequestsCollection(Action<IList<IPedestrianRequestsInfo>> modificationDelegate)
        {
            _pedestrianRequests.ModifyCollection(modificationDelegate);
        }

        public ActionResult Authenticate(IPersonInfo info)
        {
            DriverInfo driverInfo = info as DriverInfo;
            if (driverInfo != null)
            {
                ActionResult addResult = _extender.Storage.AddDriver(driverInfo);
                if (!addResult.IsValid)
                    return addResult;
                _drivers.ModifyCollection(col => col.Add(driverInfo));
                return ActionResult.ValidResult;
            }
            PedestrianInfo pedestrianInfo = info as PedestrianInfo;
            if (pedestrianInfo != null)
            {
                ActionResult addResult = _extender.Storage.AddPedestrian(pedestrianInfo);
                if (!addResult.IsValid)
                    return addResult;
                _pedestrians.ModifyCollection(col => col.Add(pedestrianInfo));
                return ActionResult.ValidResult;
            }
            return ActionResult.GetErrorResult(new NotSupportedException());
        }

        public void LogOut(Guid personId)
        {
            DriverInfo driverInfo = _drivers.Items.FirstOrDefault(d => d.Id == personId);
            if (driverInfo != null)
            {
                _drivers.ModifyCollection(col => col.Remove(driverInfo));
                _extender.Storage.RemoveDriver(driverInfo);
            }
        }
    }
}
