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

        public IEnumerable<ICityInfo> EnumerateCities()
        {
            IList<CityDA> cities = _dataProxy.Session.CreateCriteria<CityDA>().List<CityDA>();
            foreach (CityDA cityData in cities)
            {
                ICityInfo city = _server.CreateCityInfo(cityData.Id);
                city.InitialLatitude = cityData.InitialLatitude;
                city.InitialLongitude = cityData.InitialLongitude;
                city.InitialZoom = cityData.InitialZoom;
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

        public IEnumerable<IDriverInfo> EnumerateDrivers(Guid cityId)
        {
            IList<DriverAccountDA> accounts = _dataProxy.Session.CreateCriteria<DriverAccountDA>().List<DriverAccountDA>();
            IList<DriverInfoDA> drivers = _dataProxy.Session.CreateCriteria<DriverInfoDA>()/*.Add(Expression.Where<PersonInfoDA>(p => p.City.Id == cityId))*/.List<DriverInfoDA>();
            foreach (var info in accounts.Join(drivers, account => account.Person.Id, driver => driver.PersonInfo.Person.Id,
                (account, driver) => new { AccountInfo = account, DriverInfo = driver }))
            {
                IDriverInfo driverInfo = _server.CreateDriverInfo(info.AccountInfo.Person.Id);
                driverInfo.PhoneNumber = info.AccountInfo.Person.PhoneNumber;
                driverInfo.SkypeNumber = info.AccountInfo.Person.SkypeNumber;
                driverInfo.CurrentLocationLatidude = info.DriverInfo.PersonInfo.Latitude;
                driverInfo.CurrentLocationLongidude = info.DriverInfo.PersonInfo.Longitude;
                driverInfo.PersonName = info.AccountInfo.PersonName;
                driverInfo.CarColor = info.AccountInfo.CarColor;
                driverInfo.CarBrand = info.AccountInfo.CarBrand;
                driverInfo.CarNumber = info.AccountInfo.CarNumber;
                yield return driverInfo;
            }
        }
    }
}
