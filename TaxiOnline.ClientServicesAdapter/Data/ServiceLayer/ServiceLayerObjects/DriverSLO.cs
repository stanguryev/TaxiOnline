using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.ServiceLayerObjects
{
    internal class DriverSLO : PersonSLO, IDriverInfo
    {
        public DriverSLO(DriverDataContract dataContract)
        {
            base.PersonId = dataContract.PersonId;
            base.IsOnline = dataContract.IsOnline;
        }
    }
}
