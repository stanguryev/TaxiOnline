using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TaxiOnline.MapEngine.Providers
{
    public class HttpTileProvider : TileProviderBase<byte[]>
    {
        private string _urlTemplate;

        public HttpTileProvider(string urlTemplate)
        {
            _urlTemplate = urlTemplate;
        }

        public override Task<byte[]> GetBitmap(Geometry.MapTile tile)
        {
            return GetBitmapImpl(tile);
        }

        private async Task<byte[]> GetBitmapImpl(Geometry.MapTile tile)
        {
            try
            {
                string url = string.Format(_urlTemplate, tile.Zoom, tile.XNumber, tile.YNumber);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                byte[] buffer = new byte[1024];
                List<byte> outResult = new List<byte>();
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream responseStream = response.GetResponseStream())
                {
                    int bytesRead = 0;
                    do
                    {
                        bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                        outResult.AddRange(buffer.Take(bytesRead));
                    }
                    while (bytesRead > 0);
                }
                return outResult.ToArray();
            }
            catch (WebException)
            {
                return null;
            }
        }
    }
}
