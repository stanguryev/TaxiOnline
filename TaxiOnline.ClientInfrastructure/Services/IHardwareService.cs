using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.ClientInfrastructure.Services
{
    public interface IHardwareService
    {
        string GetDeviceId();
        ActionResult<MapPoint> GetCurrentLocation();
        ActionResult PhoneCall(string number);
    }
}
