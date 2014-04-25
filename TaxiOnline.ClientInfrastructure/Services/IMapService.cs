using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.ServicesEntities.Map;

namespace TaxiOnline.ClientInfrastructure.Services
{
    public interface IMapService
    {
        IMap Map { get; }
    }
}
