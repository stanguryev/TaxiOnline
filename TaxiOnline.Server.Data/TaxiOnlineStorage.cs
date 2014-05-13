using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.Server.Data.DataAccess;
using TaxiOnline.ServerInfrastructure;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;
using TaxiOnline.Toolkit.Events;

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
            IList<PedestrianInfoDA> pedestrians = _dataProxy.Session.CreateCriteria<PedestrianInfoDA>()/*.Add(Restrictions.Where<PedestrianInfoDA>(p => p.PersonInfo.City.Id == cityId))*/.List<PedestrianInfoDA>();
            foreach (var info in accounts.Join(pedestrians, account => account.Person.Id, pedestrian => pedestrian.PersonInfo.Person.Id,
                (account, pedestrian) => new { AccountInfo = account, PedestrianInfo = pedestrian }).Where(i => i.PedestrianInfo.PersonInfo.City.Id == cityId).ToArray())
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
            IList<DriverInfoDA> drivers = _dataProxy.Session.CreateCriteria<DriverInfoDA>()/*.Add(Restrictions.Where<DriverInfoDA>(p => p.PersonInfo.City.Id == cityId))*/.List<DriverInfoDA>();
            foreach (var info in accounts.Join(drivers, account => account.Person.Id, driver => driver.PersonInfo.Person.Id,
                (account, driver) => new { AccountInfo = account, DriverInfo = driver }).Where(i => i.DriverInfo.PersonInfo.City.Id == cityId).ToArray())
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

        public IEnumerable<IPedestrianRequestInfo> EnumeratePedestrianRequests(Guid cityId)
        {
            IList<PedestrianRequestDA> requests = _dataProxy.Session.CreateCriteria<PedestrianRequestDA>()/*.Add(Restrictions.Where<PedestrianRequestDA>(p => p.Author.PersonInfo.City.Id == cityId))*/.List<PedestrianRequestDA>();
            foreach (PedestrianRequestDA request in requests.Where(r => r.Author.PersonInfo.City.Id == cityId).ToArray())
            {
                IPedestrianRequestInfo requestInfo = _server.CreatePedestrianRequestInfo(request.Id, request.Author.PersonInfo.Person.Id, request.Target.PersonInfo.Person.Id);
                requestInfo.Comment = request.Comment;
                yield return requestInfo;
            }
        }

        public IEnumerable<IDriverResponseInfo> EnumerateDriverResponses(Guid cityId)
        {
            IList<DriverResponseDA> responses = _dataProxy.Session.CreateCriteria<DriverResponseDA>()/*.Add(Restrictions.Where<DriverResponseDA>(p => p.Author.PersonInfo.City.Id == cityId))*/.List<DriverResponseDA>();
            foreach (DriverResponseDA response in responses.Where(r => r.Request.Author.PersonInfo.City.Id == cityId).ToArray())
            {
                IDriverResponseInfo responseInfo = _server.CreateDriverResponseInfo(response.Id, response.Id);
                responseInfo.IsAccepted = response.IsAccepted;
                yield return responseInfo;
            }
        }

        public ActionResult AddDriver(IDriverInfo driverInfo)
        {
            PersonAccountDA account = GetPersonAccount(driverInfo);
            DriverAccountDA driverAccount = new DriverAccountDA
            {
                Person = account,
                PersonName = driverInfo.PersonName,
                CarBrand = driverInfo.CarBrand,
                CarColor = driverInfo.CarColor,
                CarNumber = driverInfo.CarNumber
            };
            DriverInfoDA driver = new DriverInfoDA
            {
                PersonInfo = new PersonInfoDA
                {
                    Latitude = driverInfo.CurrentLocationLatidude,
                    Longitude = driverInfo.CurrentLocationLongidude,
                    Person = account
                }
            };
            _dataProxy.Session.Save(driver.PersonInfo);
            _dataProxy.Session.Save(driverAccount);
            _dataProxy.Session.Save(driver);
            return ActionResult.ValidResult;
        }

        public ActionResult AddPedestrian(IPedestrianInfo pedestrianInfo)
        {
            throw new NotImplementedException();
        }

        public ActionResult RemoveDriver(IDriverInfo driverInfo)
        {
            DriverInfoDA data = _dataProxy.Session.CreateCriteria<DriverInfoDA>().List<DriverInfoDA>().FirstOrDefault(p => p.PersonInfo.Person.Id == driverInfo.Id);
            if (data == null)
                return ActionResult.GetErrorResult(new KeyNotFoundException());
            _dataProxy.Session.Delete(data);
            _dataProxy.Session.Delete(data.PersonInfo);
            return ActionResult.ValidResult;
        }

        public ActionResult RemovePedestrian(IPedestrianInfo pedestrianInfo)
        {
            throw new NotImplementedException();
        }

        private PersonAccountDA GetPersonAccount(IPersonInfo personInfo)
        {
            PersonAccountDA account = _dataProxy.Session.CreateCriteria<PersonAccountDA>().List<PersonAccountDA>().FirstOrDefault(p => p.Id == personInfo.Id);
            if (account == null)
            {
                account = new PersonAccountDA
                {
                    Id = personInfo.Id,
                    PhoneNumber = personInfo.PhoneNumber,
                    SkypeNumber = personInfo.SkypeNumber
                };
                _dataProxy.Session.Save(account);
            }
            return account;
        }
    }
}
