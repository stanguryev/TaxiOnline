using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.ServiceLayerObjects
{
    internal class DriverAuthenticationRequestSLO : AuthenticationRequestSLO, IDriverAuthenticationRequest
    {
        private string _personName;
        private string _carColor;
        private string _carBrand;
        private string _carNumber;

        public string PersonName
        {
            get { return _personName; }
            set { _personName = value; }
        }

        public string CarColor
        {
            get { return _carColor; }
            set { _carColor = value; }
        }

        public string CarBrand
        {
            get { return _carBrand; }
            set { _carBrand = value; }
        }

        public string CarNumber
        {
            get { return _carNumber; }
            set { _carNumber = value; }
        }

        public DriverAuthenticationRequestSLO(Guid cityId)
            : base(cityId)
        {

        }

        public override AuthenticationRequestDataContract CreateDataContract()
        {
            return new DriverAuthenticationRequestDataContract
            {
                CurrentLocationLatidude = CurrentLocation.Latitude,
                CurrentLocationLongitude = CurrentLocation.Longitude,
                PhoneNumber = PhoneNumber,
                SkypeNumber = SkypeNumber,
                PersonName = PersonName,
                CarColor = CarColor,
                CarBrand = CarBrand,
                CarNumber = CarNumber
            };
        }
    }
}
