using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.ServicesEntities.Settings;

namespace TaxiOnline.ClientServicesAdapter.Settings
{
    public class SettingsObject : ISettings
    {
        private string _serverEndpointAddress;
        private MapMode _mapMode;

        public string ServerEndpointAddress
        {
            get { return _serverEndpointAddress; }
            set { _serverEndpointAddress = value; }
        }

        public MapMode MapMode
        {
            get { return _mapMode; }
            set { _mapMode = value; }
        }
    }
}
