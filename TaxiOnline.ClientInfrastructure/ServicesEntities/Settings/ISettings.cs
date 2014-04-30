using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.ClientInfrastructure.ServicesEntities.Settings
{
    public interface ISettings
    {
        string ServerEndpointAddress { get; set; }
        MapMode MapMode { get; set; }
    }
}
