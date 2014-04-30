using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.Services;
using TaxiOnline.ClientInfrastructure.ServicesEntities.Map;

namespace TaxiOnline.ClientServicesAdapter.Map
{
    public abstract class MapAdapter : IMapService
    {
        protected readonly MapWrapperBase _map;
        protected MapSourceManagerBase _mapSourceManager;

        public IMap Map
        {
            get { return _map; }
        }

        public MapAdapter(MapMode mode)
        {
            _map = CreateMapWrapper();
            _mapSourceManager = CreateMapSourceManager(mode, _map);
        }

        protected abstract MapWrapperBase CreateMapWrapper();

        protected MapSourceManagerBase CreateMapSourceManager(MapMode mode, MapWrapperBase mapWrapper)
        {
            switch (mode)
            {
                case MapMode.Online:
                    return new OnlineMapSourceManager(mapWrapper);
                    break;
                case MapMode.Cached:
                    return new CacheMapSourceManager(mapWrapper);
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
        }
    }
}
