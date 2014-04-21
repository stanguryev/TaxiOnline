using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Factories;
using TaxiOnline.ClientInfrastructure.Services;

namespace TaxiOnline.ClientServicesAdapter
{
    public abstract class ServicesFactoryBase : IServicesFactory
    {
        private readonly Lazy<IDataService> _dataService;
        private string _serverEndpointAddress;

        public ServicesFactoryBase()
        {
            _dataService = new Lazy<IDataService>(() => new TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.DataServiceImpl(_serverEndpointAddress), true);
        }

        public abstract IMapService GetCurrentMapService();
        
        public abstract IHardwareService GetCurrentHardwareService();

        public void ConfigureDataService(string serverEndpointAddress)
        {
            _serverEndpointAddress = serverEndpointAddress;
        }

        public IDataService GetCurrentDataService()
        {
            return _dataService.Value;
        }
    }
}
