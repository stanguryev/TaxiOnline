using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.Server.Core.Objects;
using TaxiOnline.ServerInfrastructure;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;
using TaxiOnline.Toolkit.Threading.CollectionsDecorators;

namespace TaxiOnline.Server.Core
{
    public class TaxiOnlineServer : ITaxiOnlineServer
    {
        private readonly Lazy<ITaxiOnlineMobileService> _mobileService;
        private readonly Lazy<ITaxiOnlineStorage> _storage;
        private Func<ITaxiOnlineServer, ITaxiOnlineMobileService> _mobileServiceInitDelegate;
        private Func<ITaxiOnlineServer, ITaxiOnlineStorage> _storageInitDelegate;
        private readonly ReadonlyCollectionDecorator<PedestrianInfo> _pedestrians = new ReadonlyCollectionDecorator<PedestrianInfo>();
        private readonly ReadonlyCollectionDecorator<IDriverInfo> _drivers = new ReadonlyCollectionDecorator<IDriverInfo>();

        public ITaxiOnlineMobileService MobileService
        {
            get { return _mobileService.Value; }
        }

        public ITaxiOnlineStorage Storage
        {
            get { return _storage.Value; }
        }

        public IEnumerable<IPedestrianInfo> Pedestrians
        {
            get { return _pedestrians.Items; }
        }

        public IEnumerable<IDriverInfo> Drivers
        {
            get { return _drivers.Items; }
        }

        public TaxiOnlineServer()
        {
            _mobileService = new Lazy<ITaxiOnlineMobileService>(() => _mobileServiceInitDelegate(this), true);
            _storage = new Lazy<ITaxiOnlineStorage>(() => _storageInitDelegate(this), true);
        }

        public void LoadPersistentState()
        {
            List<IPedestrianInfo> pedestrians = _storage.Value.EnumeratePedestrians(Guid.Empty).ToList();
            _pedestrians.ModifyCollection<IPedestrianInfo>(col => pedestrians.ForEach(p => col.Add(p)));
        }

        public void InitMobileService(Func<ITaxiOnlineServer, ITaxiOnlineMobileService> mobileServiceInitDelegate)
        {
            _mobileServiceInitDelegate = mobileServiceInitDelegate;
        }

        public void InitStorage(Func<ITaxiOnlineServer, ITaxiOnlineStorage> storageInitDelegate)
        {
            _storageInitDelegate = storageInitDelegate;
        }

        public IEnumerable<ICityInfo> EnumerateCities(string userCultureName)
        {
            return _storage.Value.EnumerateCities(userCultureName);
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

        public IDriverInfo CreateDriverInfo()
        {
            return new DriverInfo(Guid.NewGuid())
            {
                IsOnline = true
            };
        }

        public void ModifyPedestriansCollection(Action<IList<IPedestrianInfo>> modificationDelegate)
        {
            _pedestrians.ModifyCollection(modificationDelegate);
        }

        public void ModifyDriversCollection(Action<IList<IDriverInfo>> modificationDelegate)
        {
            _drivers.ModifyCollection(modificationDelegate);
        }


    }
}
