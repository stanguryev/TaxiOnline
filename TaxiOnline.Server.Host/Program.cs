using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.Server.Core;
using TaxiOnline.Server.Data;
using TaxiOnline.Server.MobileService;
using TaxiOnline.ServerInfrastructure;

namespace TaxiOnline.Server.Host
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TaxiOnlineServer server = new TaxiOnlineServer();
            server.InitStorage(srv => new TaxiOnlineStorage(srv));
            server.InitMobileService(srv => new TaxiOnlineSerivceHost(srv));
            ITaxiOnlineStorage storage = server.Storage;
            ITaxiOnlineMobileService mobileService = server.MobileService;
            mobileService.Open();
            Console.ReadLine();
        }
    }
}
