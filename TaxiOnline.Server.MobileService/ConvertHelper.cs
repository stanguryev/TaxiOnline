using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.Server.MobileService
{
    internal class ConvertHelper
    {
        public static CityDataContract CreateCityDataContract(ServerInfrastructure.EntitiesInterfaces.ICityInfo city)
        {
            return new CityDataContract
            {
                Id = city.Id,
                Name = city.Name,
                InitialLatitude = city.InitialLatitude,
                InitialLongitude = city.InitialLongitude,
                InitialZoom = city.InitialZoom
            };
        }
    }
}
