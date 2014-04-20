using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiOnline.ServerInfrastructure.EntitiesInterfaces
{
    public interface ICityInfo
    {
        Guid Id { get; }
        string Name { get; set; }
        double InitialLatitude { get; set; }
        double InitialLongitude { get; set; }
        double InitialZoom { get; set; }
    }
}
