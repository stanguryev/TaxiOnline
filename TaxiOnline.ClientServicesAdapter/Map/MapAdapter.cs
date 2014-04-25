using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Services;
using TaxiOnline.ClientInfrastructure.ServicesEntities.Map;

namespace TaxiOnline.ClientServicesAdapter.Map
{
    public abstract class MapAdapter : IMapService
    {
        protected readonly MapWrapperBase _map;

        public IMap Map
        {
            get { return _map; }
        }

        public MapAdapter()
        {
            _map = CreateMapWrapper();
        }

        protected abstract MapWrapperBase CreateMapWrapper();
    }
}
