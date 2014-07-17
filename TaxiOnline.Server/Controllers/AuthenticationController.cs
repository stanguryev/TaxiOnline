using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using TaxiOnline.Server.DataObjects;
using TaxiOnline.Server.Models;
using System.Threading.Tasks;

namespace TaxiOnline.Server.Controllers
{
    public class AuthenticationController : ApiController
    {
        public ApiServices Services { get; set; }

        [Route("api/Authentication/AuthenticateAsPedestrian")]
        [HttpPost]
        public IHttpActionResult AuthenticateAsPedestrian(PedestrianAuthenticationDTO authenticationInfo)
        {
            InteractionModel interactionModel = InteractionModel.Instance;
            CityModel cityModel = interactionModel.Cities.FirstOrDefault(city => city.Id == authenticationInfo.CityId);
            if (cityModel == null)
                return InternalServerError();
            PedestrianDTO outResult = new PedestrianDTO
            {
                Id = Guid.NewGuid().ToString(),
                Latitude = authenticationInfo.Latitude,
                Longitude = authenticationInfo.Longitude,
                PhoneNumber = authenticationInfo.PhoneNumber,
                SkypeNumber = authenticationInfo.SkypeNumber
            };
            cityModel.AddPedestrian(outResult);
            return Ok<PedestrianDTO>(outResult);
        }

        [Route("api/Authentication/AuthenticateAsDriver")]
        [HttpPost]
        public IHttpActionResult AuthenticateAsDriver(DriverAuthenticationDTO authenticationInfo)
        {
            InteractionModel interactionModel = InteractionModel.Instance;
            CityModel cityModel = interactionModel.Cities.FirstOrDefault(city => city.Id == authenticationInfo.CityId);
            if (cityModel == null)
                return InternalServerError();
            return Ok<DriverDTO>(new DriverDTO
            {
                Id = Guid.NewGuid().ToString(),
                Latitude = authenticationInfo.Latitude,
                Longitude = authenticationInfo.Longitude,
                PhoneNumber = authenticationInfo.PhoneNumber,
                SkypeNumber = authenticationInfo.SkypeNumber,
                PersonName = authenticationInfo.PersonName,
                CarBrand = authenticationInfo.CarBrand,
                CarColor = authenticationInfo.CarColor,
                CarNumber = authenticationInfo.CarNumber
            });
        }
    }
}
