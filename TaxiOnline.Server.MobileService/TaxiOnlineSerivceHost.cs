using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ServerInfrastructure;
using TaxiOnline.ServiceContract;

namespace TaxiOnline.Server.MobileService
{
    public class TaxiOnlineSerivceHost : ServiceHost, ITaxiOnlineMobileService
    {
        private const string EndPointPath="TaxiOnline";

        public TaxiOnlineSerivceHost(ITaxiOnlineServer server)
            : base(new TaxiOnlineService(server), GetEndPointAddresses())
        {
            AddServiceEndpoint(typeof(ITaxiOnlineService), new BasicHttpBinding(BasicHttpSecurityMode.None), GetEndPointAddresses().First());
        }

        private static Uri[] GetEndPointAddresses()
        {
            IPAddress[] address = Dns.GetHostAddresses(Dns.GetHostName());
            return address.Where(addr => addr.AddressFamily == AddressFamily.InterNetwork).Select(addr => new Uri(new Uri(string.Format("http://{0}", addr)), EndPointPath)).ToArray();
        }
    }
}
