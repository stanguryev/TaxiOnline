using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.ClientInfrastructure.ServicesEntities.DataService
{
    public interface IDriverAuthenticationRequest : IAuthenticationRequest
    {
        string CarColor { get; set; }
    }
}
