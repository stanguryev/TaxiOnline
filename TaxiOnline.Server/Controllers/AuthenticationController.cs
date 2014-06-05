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
            return Ok();
        }

        [Route("api/Authentication/AuthenticateAsDriver")]
        [HttpPost]
        public IHttpActionResult AuthenticateAsDriver(DriverAuthenticationDTO authenticationInfo)
        {
            InteractionModel interactionModel = InteractionModel.Instance;
            CityModel cityModel = interactionModel.Cities.FirstOrDefault(city => city.Id == authenticationInfo.CityId);
            if (cityModel == null)
                return InternalServerError();
            return Ok();
        }
    }
}
