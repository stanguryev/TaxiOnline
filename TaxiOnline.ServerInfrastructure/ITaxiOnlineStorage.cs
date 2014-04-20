using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;

namespace TaxiOnline.ServerInfrastructure
{
    public interface ITaxiOnlineStorage
    {
        IEnumerable<ICityInfo> EnumerateCities(string userCultureName);
    }
}
