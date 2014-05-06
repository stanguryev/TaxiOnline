using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.ServerInfrastructure.EntitiesInterfaces
{
    public interface IPedestrianRequestsInfo
    {
        Guid Id { get; }
        Guid DriverId { get; }
        Guid PedestrianId { get; }
    }
}
