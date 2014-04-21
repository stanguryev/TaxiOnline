using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.ClientInfrastructure.Services
{
    public interface IHardwareService
    {
        string GetDeviceId();
        MapPoint GetCurrentLocation();
    }
}
