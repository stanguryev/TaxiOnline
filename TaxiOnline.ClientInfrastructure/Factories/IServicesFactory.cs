using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Services;

namespace TaxiOnline.ClientInfrastructure.Factories
{
    public interface IServicesFactory
    {
        IMapService GetCurrentMapService();
        IDataService GetCurrentDataService();
    }
}
