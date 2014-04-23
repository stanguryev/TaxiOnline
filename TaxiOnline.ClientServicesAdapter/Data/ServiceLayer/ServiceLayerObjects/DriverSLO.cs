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
        private string _personName;
        private string _carColor;
        private string _carBrand;
        private string _carNumber;

        public string PersonName
        {
            get { return _personName; }
            internal set { _personName = value; }
        }

        public string CarColor
        {
            get { return _carColor; }
            internal set { _carColor = value; }
        }

        public string CarBrand
        {
            get { return _carBrand; }
            internal set { _carBrand = value; }
        }

        public string CarNumber
        {
            get { return _carNumber; }
            internal set { _carNumber = value; }
        }

        public DriverSLO(DriverDataContract dataContract)
            : base(dataContract)
        {
            _personName = dataContract.PersonName;
            _carColor = dataContract.CarColor;
            _carBrand = dataContract.CarBrand;
            _carNumber = dataContract.CarNumber;
        }
    }
}
