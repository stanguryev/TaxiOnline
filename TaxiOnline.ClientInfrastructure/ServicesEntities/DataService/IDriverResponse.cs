using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.ClientInfrastructure.ServicesEntities.DataService
{
    public interface IDriverResponse
    {
        Guid Id { get; }
        Guid DriverId { get; }
        Guid RequestId { get; }
        DriverResponseState State { get; set; }
    }
}
