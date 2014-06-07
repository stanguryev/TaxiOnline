using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using TaxiOnline.Server.DataObjects;
using TaxiOnline.Server.DataAccess;

namespace TaxiOnline.Server.Controllers
{
    public class DictionaryController : ApiController
    {
        public ApiServices Services { get; set; }

        // GET api/Dictionary
        public string Get()
        {
            return string.Empty;
        }

        [Route("api/Dictionary/EnumerateCities/{cultureName}")]
        public IEnumerable<CityDTO> EnumerateCities(string cultureName)
        {
            return EnumerateCitiesImpl(cultureName).ToArray();
        }

        public IEnumerable<CityDTO> EnumerateCitiesImpl(string cultureName)
        {
            using (DatabaseModel dbContext = new DatabaseModel())
            {
                foreach (City city in dbContext.Cities.ToArray())
                {
                    CityName cityName = city.CityNames.FirstOrDefault(n => n.CultureName == cultureName);
                    if (cityName == null)
                        continue;
                    yield return new CityDTO
                    {
                        Id = city.Id,
                        CityName = cityName.Name,
                        InitialLatitude = city.InitialLatitude,
                        InitialLongitude = city.InitialLongitude,
                        InitialZoom = city.InitialZoom,
                        PhoneConstraintPattern = city.PhoneConstraintPattern,
                        PhoneCorrectionPattern = city.PhoneCorrectionPattern,
                        PhoneConstraintDescription = cityName.PhoneConstraintDescription
                    };
                }
            }
        }

    }
}
