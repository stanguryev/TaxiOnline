using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.ClientInfrastructure.ServicesEntities.DataService
{
    public interface ICityInfo
    {
        Guid Id { get; }
        string Name { get; }
        double InitialLatitude { get; }
        double InitialLongitude { get; }
        double InitialZoom { get; }
    }
}
