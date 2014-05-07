using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.ServerInfrastructure.EntitiesInterfaces
{
    public interface IDriverResponseInfo
    {
        Guid Id { get; }
        Guid RequestId { get; }
        bool IsAccepted { get; set; }
    }
}
