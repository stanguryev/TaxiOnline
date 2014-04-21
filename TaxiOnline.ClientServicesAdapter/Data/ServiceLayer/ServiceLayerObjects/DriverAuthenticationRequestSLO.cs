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
        private string _carColor;

        public string CarColor
        {
            get { return _carColor; }
            set { _carColor = value; }
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
                CarColor = CarColor
            };
        }
    }
}
