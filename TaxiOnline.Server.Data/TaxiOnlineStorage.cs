using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.Server.Data.DataAccess;
using TaxiOnline.ServerInfrastructure;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;

namespace TaxiOnline.Server.Data
{
    public class TaxiOnlineStorage : ITaxiOnlineStorage
    {
        private readonly ITaxiOnlineServer _server;
        private DataProxy _dataProxy;

        public TaxiOnlineStorage(ITaxiOnlineServer server)
        {
            _server = server;
            _dataProxy = new DataProxy();
        }

        public IEnumerable<ICityInfo> EnumerateCities(string userCultureName)
        {
            IList<CityNameDA> cityNames = _dataProxy.Session.CreateCriteria<CityNameDA>().Add(Expression.Where<CityNameDA>(name => name.CultureName == userCultureName)).List<CityNameDA>();
            foreach (IGrouping<Guid, CityNameDA> cityNameGroup in cityNames.GroupBy(name => name.City.Id).ToArray())
            {
                ICityInfo city = _server.CreateCityInfo(cityNameGroup.Key);
                CityNameDA cityName = cityNameGroup.Single();
                city.Name = cityName.Name;
                city.InitialLatitude = cityName.City.InitialLatitude;
                city.InitialLongitude = cityName.City.InitialLongitude;
                city.InitialZoom = cityName.City.InitialZoom;
                yield return city;
            }
        }
    }
}
