using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;

namespace TaxiOnline.ServerInfrastructure
{
    public interface ITaxiOnlineServer
    {
        IEnumerable<ICityInfo> EnumerateCities(string userCultureName);

        ICityInfo CreateCityInfo(Guid guid);
    }
}
