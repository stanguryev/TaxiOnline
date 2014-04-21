using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.ServiceLayerObjects
{
    internal class DriverSLO : PersonSLO, IDriverInfo
    {
        private string _carColor;

        public string CarColor
        {
            get { return _carColor; }
            internal set { _carColor = value; }
        }

        public DriverSLO(DriverDataContract dataContract)
            : base(dataContract)
        {
            _carColor = dataContract.CarColor;
        }

    }
}
