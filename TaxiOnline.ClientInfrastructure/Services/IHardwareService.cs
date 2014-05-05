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
        event EventHandler<ValueEventArgs<MapPoint>> LocationChanged;
        string GetDeviceId();
        ActionResult<MapPoint> GetCurrentLocation();
        ActionResult PhoneCall(string number);
    }
}
