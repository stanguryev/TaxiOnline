using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.ClientServicesAdapter.Map
{
    internal class CacheMapSourceManager : MapSourceManagerBase
    {
        public CacheMapSourceManager(MapWrapperBase mapWrapper)
            : base(mapWrapper)
        {

        }

        protected override void ApplyToMapWrapper(MapWrapperBase mapWrapper)
        {
            OsmSharp.Osm.Data.DataSourceReadOnlyBase dataSource = null;// new OsmSharp.Data.SQLLite.Osm.SQLLiteDataSource()
            OsmSharp.UI.Map.Styles.StyleInterpreter styleInterpreter = new OsmSharp.UI.Map.Styles.MapCSS.MapCSSInterpreter(
                string.Empty, new OsmSharp.UI.Map.Styles.MapCSS.MapCSSDictionaryImageSource());
            mapWrapper.Map.AddLayer(new OsmSharp.UI.Map.Layers.LayerOsm(dataSource, styleInterpreter, mapWrapper.Map.Projection));
        }
    }
}
