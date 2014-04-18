using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.ClientInfrastructure.ServicesEntities.DataService
{
    public interface IPersonInfo
    {
        Guid PersonId { get; }
        bool IsOnline { get; }
    }
}
