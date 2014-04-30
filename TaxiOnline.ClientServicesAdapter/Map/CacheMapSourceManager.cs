using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.ClientServicesAdapter.Map
{
    public class CacheMapSourceManager : MapSourceManagerBase
    {
        public CacheMapSourceManager(MapWrapperBase mapWrapper)
            : base(mapWrapper)
        {

        }

        protected override void ApplyToMapWrapper(MapWrapperBase mapWrapper)
        {
            //OsmSharp.Osm.Data.DataSourceReadOnlyBase dataSource = new OsmSharp.Data.SQLite.Osm.SQLiteDataSource("");
            //OsmSharp.UI.Map.Styles.StyleInterpreter styleInterpreter = new OsmSharp.UI.Map.Styles.MapCSS.MapCSSInterpreter(
            //    GetMapCss(), new OsmSharp.UI.Map.Styles.MapCSS.MapCSSDictionaryImageSource());
            //mapWrapper.Map.AddLayer(new OsmSharp.UI.Map.Layers.LayerOsm(dataSource, styleInterpreter, mapWrapper.Map.Projection));
        }

#if false

        protected void LoadPbfFile(string url, Action<double> progressCallback)
        {
            System.Net.WebRequest request = System.Net.HttpWebRequest.CreateHttp(url);
            using (System.Net.WebResponse response = request.EndGetResponse(request.BeginGetResponse(ar => { }, null)))
            {
                long contentLength = response.ContentLength;
                long loadedContentLength = 0;
                using (System.IO.Stream responseStream = response.GetResponseStream())
                using (System.IO.Stream targetStream = GetDownloadCacheStream())
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        targetStream.Write(buffer, 0, bytesRead);
                        loadedContentLength += bytesRead;
                        progressCallback((double)loadedContentLength / (double)contentLength);
                    }
                }
            }
        }

        protected void ProceedPbfFile()
        {
            OsmSharp.Data.SQLite.Osm.Streams.SQLiteOsmStreamTarget cacheTarget = new OsmSharp.Data.SQLite.Osm.Streams.SQLiteOsmStreamTarget("");
            try
            {
                using (System.IO.Stream pbfCacheStream = GetDownloadCacheStream())
                using (OsmSharp.Osm.PBF.Streams.PBFOsmStreamSource pbfStream = new OsmSharp.Osm.PBF.Streams.PBFOsmStreamSource(pbfCacheStream))
                {
                    cacheTarget.Initialize();
                    cacheTarget.RegisterSource(pbfStream);
                    cacheTarget.Pull();
                    cacheTarget.Flush();
                }
            }
            finally
            {
                cacheTarget.Close();
            }
        }

        protected abstract System.IO.Stream GetDownloadCacheStream();
#endif
    }
}
