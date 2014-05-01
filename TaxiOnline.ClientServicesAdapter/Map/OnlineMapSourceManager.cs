using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.ClientServicesAdapter.Map
{
    internal class OnlineMapSourceManager : MapSourceManagerBase
    {
        public OnlineMapSourceManager(MapWrapperBase mapWrapper)
            : base(mapWrapper)
        {

        }

        protected override void ApplyToMapWrapper(MapWrapperBase mapWrapper)
        {
            mapWrapper.Map.AddLayer(new OsmSharp.UI.Map.Layers.LayerTile(@"http://otile1.mqcdn.com/tiles/1.0.0/osm/{0}/{1}/{2}.png"));
            //mapWrapper.Map.AddLayer(new OsmSharp.UI.Map.Layers.LayerTile(@"http://tiles.openseamap.org/seamark/{0}/{1}/{2}.png"));
        }
    }
}
