using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.ClientInfrastructure.ServicesEntities.DataService
{
    public interface IDriverAuthenticationRequest : IAuthenticationRequest
    {
        string PersonName { get; set; }
        string CarColor { get; set; }
        string CarBrand { get; set; }
        string CarNumber { get; set; }
    }
}
