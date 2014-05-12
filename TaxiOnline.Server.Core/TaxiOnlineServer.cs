using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.Server.Core.Logic;
using TaxiOnline.Server.Core.Objects;
using TaxiOnline.ServerInfrastructure;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;
using TaxiOnline.ServerInfrastructure.LogicInterfaces;
using TaxiOnline.Toolkit.Threading.CollectionsDecorators;

namespace TaxiOnline.Server.Core
{
    public class TaxiOnlineServer : ITaxiOnlineServer
    {
        private LogicExtender _extender;
        private readonly Lazy<ITaxiOnlineMobileService> _mobileService;
        private Func<ITaxiOnlineServer, ITaxiOnlineMobileService> _mobileServiceInitDelegate;
        private readonly ReadonlyCollectionDecorator<CityLogic> _cities = new ReadonlyCollectionDecorator<CityLogic>();

        public ITaxiOnlineMobileService MobileService
        {
            get { return _mobileService.Value; }
        }

        public ITaxiOnlineStorage Storage
        {
            get { return _extender.Storage; }
        }

        public IEnumerable<ICityLogic> Cities
        {
            get { return _cities.Items; }
        }

        public TaxiOnlineServer()
        {
            _extender = new LogicExtender(this);
            _mobileService = new Lazy<ITaxiOnlineMobileService>(() => _mobileServiceInitDelegate(this), true);
        }

        public void LoadPersistentState()
        {
            foreach (CityLogic city in _extender.Storage.EnumerateCities().Select(i => new CityLogic((CityInfo)i, this, _extender)).ToArray())
            {
                city.LoadPersistentState();
                _cities.ModifyCollection(col => col.Add(city));
            }
        }

        public void InitMobileService(Func<ITaxiOnlineServer, ITaxiOnlineMobileService> mobileServiceInitDelegate)
        {
            _mobileServiceInitDelegate = mobileServiceInitDelegate;
        }

        public void InitStorage(Func<ITaxiOnlineServer, ITaxiOnlineStorage> storageInitDelegate)
        {
            _extender.InitStorage(storageInitDelegate);
        }

        public IEnumerable<ICityInfo> EnumerateCities(string userCultureName)
        {
            return _extender.Storage.EnumerateCities(userCultureName);
        }

        public void AuthenticateAsPedestrian(IPedestrianInfo pedestrianInfo, Guid cityId)
        {
            CityLogic city = _cities.Items.FirstOrDefault(c => c.Info.Id == cityId);
            if (city != null)
                city.ModifyPedestriansCollection(col => col.Add(pedestrianInfo));
        }

        public void AuthenticateAsDriver(IDriverInfo driverInfo, Guid cityId)
        {
            CityLogic city = _cities.Items.FirstOrDefault(c => c.Info.Id == cityId);
            if (city != null)
                city.ModifyDriversCollection(col => col.Add(driverInfo));
        }

        public ICityInfo CreateCityInfo(Guid id)
        {
            return new CityInfo(id);
        }

        public IPedestrianInfo CreatePedestrianInfo(Guid id)
        {
            return new PedestrianInfo(id)
            {
                IsOnline = true
            };
        }

        public IPedestrianInfo CreatePedestrianInfo()
        {
            return CreatePedestrianInfo(Guid.NewGuid());
        }

        public IDriverInfo CreateDriverInfo(Guid id)
        {
            return new DriverInfo(id)
            {
                IsOnline = true
            };
        }

        public IDriverInfo CreateDriverInfo()
        {
            return CreateDriverInfo(Guid.NewGuid());
        }

        public IPedestrianRequestInfo CreatePedestrianRequestInfo(IPedestrianInfo pedestrian, IDriverInfo driver)
        {
            return new PedestrianRequestInfo(Guid.NewGuid(), driver.Id, pedestrian.Id);
        }

        public IPedestrianRequestInfo CreatePedestrianRequestInfo(Guid id, Guid pedestrianId, Guid driverId)
        {
            return new PedestrianRequestInfo(id, driverId, pedestrianId);
        }

        public void PushPedestrianRequestInfo(IPedestrianRequestInfo requestInfo)
        {
            CityLogic city = _cities.Items.FirstOrDefault(c => c.Pedestrians.Any(p => p.Id == requestInfo.PedestrianId));
            if (city != null)
                city.ModifyPedestrianRequestsCollection(col => col.Add(requestInfo));
        }
    }
}
