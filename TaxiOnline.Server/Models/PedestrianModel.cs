using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.Server.DataAccess;
using TaxiOnline.Server.DataObjects;

namespace TaxiOnline.Server.Models
{
    public class PedestrianModel
    {
        private readonly PedestriansInfo _pedestrianDA;
        private readonly CityModel _city;

        public Guid Id
        {
            get { return _pedestrianDA.PersonsInfo.PersonId; }
        }

        public string PhoneNumber
        {
            get { return _pedestrianDA.PersonsInfo.PersonAccount.PhoneNumber; }
        }

        public string SkypeNumber
        {
            get { return _pedestrianDA.PersonsInfo.PersonAccount.SkypeNumber; }
        }

        public Guid CityId
        {
            get { return _city.Id; }
        }

        public double? Latitude
        {
            get { return _pedestrianDA.PersonsInfo.Latitude; }
        }

        public double? Longitude
        {
            get { return _pedestrianDA.PersonsInfo.Longitude; }
        }

        public double? Altitude
        {
            get { return _pedestrianDA.PersonsInfo.Altitude; }
        }

        public CityModel City
        {
            get { return _city; }
        }

        public PedestrianModel(CityModel city, PedestriansInfo pedestrianDA)
        {
            _city = city;
            _pedestrianDA = pedestrianDA;
        }

        public PedestrianModel(CityModel city, PedestrianDTO pedestrianDTO)
            : this(city, SaveDtoToDb(pedestrianDTO))
        {

        }

        public void LoadDb()
        {
            
        }

        public static PedestriansInfo SaveDtoToDb(PedestrianDTO pedestrianDTO)
        {
            using (DatabaseModel dbContext = new DatabaseModel())
            {
                PedestriansInfo existingInfo = dbContext.Set<PedestriansInfo>().First(i => i.PersonsInfo.PersonId == Guid.Parse(pedestrianDTO.Id));
                if (existingInfo != null)
                    return existingInfo;
                PersonAccount account = new PersonAccount
                {
                    Id = Guid.Parse(pedestrianDTO.Id),
                    PhoneNumber = pedestrianDTO.PhoneNumber,
                    SkypeNumber = pedestrianDTO.SkypeNumber
                };
                dbContext.Set<PersonAccount>().Attach(account);
                dbContext.Set<PersonAccount>().Add(account);
                PersonsInfo personInfo = new PersonsInfo
                {
                    Altitude = pedestrianDTO.Altitude,
                    Longitude = pedestrianDTO.Longitude,
                    Latitude = pedestrianDTO.Latitude,
                    CityId = pedestrianDTO.CityId,
                    PersonId = account.Id
                };
                dbContext.Set<PersonsInfo>().Attach(personInfo);
                dbContext.Set<PersonsInfo>().Add(personInfo);
                PedestrianAccount pedestrianAccount = new PedestrianAccount
                {
                    PersonId = account.Id
                };
                dbContext.Set<PedestrianAccount>().Attach(pedestrianAccount);
                dbContext.Set<PedestrianAccount>().Add(pedestrianAccount);
                PedestriansInfo pedestrianInfo = new PedestriansInfo
                {
                    PersonInfo = personInfo.Id                    
                };
                dbContext.Set<PedestriansInfo>().Attach(pedestrianInfo);
                dbContext.Set<PedestriansInfo>().Add(pedestrianInfo);
                dbContext.SaveChanges();
                return pedestrianInfo;
            }
        }
    }
}
