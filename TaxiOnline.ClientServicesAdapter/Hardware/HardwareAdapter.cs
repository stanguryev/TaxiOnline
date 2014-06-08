using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.Services;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.ClientServicesAdapter.Hardware
{
    public abstract class HardwareAdapter : IHardwareService
    {
        public abstract event EventHandler<ValueEventArgs<MapPoint>> LocationChanged;
        public abstract event EventHandler<ValueEventArgs<string>> IncomingCallArrived;

        public abstract string GetDeviceId();
        public abstract ActionResult<MapPoint> GetCurrentLocation();
        //public abstract void RequestLocation();
        public abstract ActionResult PhoneCall(string number);
    }
}
