using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.ClientServicesAdapter.Map
{
    public abstract class MapSourceManagerBase
    {
        private readonly MapWrapperBase _mapWrapper;

        public MapSourceManagerBase(MapWrapperBase mapWrapper)
        {
            _mapWrapper = mapWrapper;
            // ApplyToMapWrapper(mapWrapper);
        }

        protected abstract void ApplyToMapWrapper(MapWrapperBase mapWrapper);
    }
}
