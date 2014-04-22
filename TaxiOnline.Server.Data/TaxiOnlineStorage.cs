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

        public IEnumerable<IPedestrianInfo> EnumeratePedestrians(Guid cityId)
        {
            IList<PedestrianAccountDA> accounts = _dataProxy.Session.CreateCriteria<PedestrianAccountDA>().List<PedestrianAccountDA>();
            IList<PedestrianInfoDA> pedestrians = _dataProxy.Session.CreateCriteria<PedestrianInfoDA>()/*.Add(Expression.Where<PersonInfoDA>(p => p.City.Id == cityId))*/.List<PedestrianInfoDA>();
            foreach (var info in accounts.Join(pedestrians, account => account.Person.Id, pedestrian => pedestrian.PersonInfo.Person.Id,
                (account, pedestrian) => new { AccountInfo = account, PedestrianInfo = pedestrian }))
            {
                IPedestrianInfo pedesrtianInfo = _server.CreatePedestrianInfo(info.AccountInfo.Person.Id);
                pedesrtianInfo.PhoneNumber = info.AccountInfo.Person.PhoneNumber;
                pedesrtianInfo.SkypeNumber = info.AccountInfo.Person.SkypeNumber;
                pedesrtianInfo.CurrentLocationLatidude = info.PedestrianInfo.PersonInfo.Latitude;
                pedesrtianInfo.CurrentLocationLongidude = info.PedestrianInfo.PersonInfo.Longitude;
                yield return pedesrtianInfo;
            }
        }
    }
}
