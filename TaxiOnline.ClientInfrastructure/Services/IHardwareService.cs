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
        event EventHandler<ValueEventArgs<string>> IncomingCallArrived;

        string GetDeviceId();
        ActionResult<MapPoint> GetCurrentLocation();
        //void RequestLocation();
        ActionResult PhoneCall(string number);
    }
}
