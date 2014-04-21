using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.Services;

namespace TaxiOnline.ClientServicesAdapter.Hardware
{
    public abstract class HardwareAdapter : IHardwareService
    {
        public abstract string GetDeviceId();

        public abstract MapPoint GetCurrentLocation();
    }
}
