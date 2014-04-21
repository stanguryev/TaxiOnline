using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.ClientInfrastructure.ServicesEntities.DataService
{
    public interface IAuthenticationRequest
    {
        string PhoneNumber { get; set; }
        string SkypeNumber { get; set; }
        MapPoint CurrentLocation { get; set; }
    }
}
