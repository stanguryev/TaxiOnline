using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.ClientInfrastructure.ServicesEntities.DataService
{
    public interface IDriverResponse
    {
        Guid Id { get; }
        Guid DriverId { get; }
        Guid RequestId { get; }
        bool IsCanceled { get; set; }
    }
}
