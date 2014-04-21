using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.ServiceLayerObjects
{
    internal class PedestrianAuthenticationRequestSLO : AuthenticationRequestSLO, IPedestrianAuthenticationRequest
    {
        public PedestrianAuthenticationRequestSLO(Guid cityId)
            : base(cityId)
        {

        }

        public override AuthenticationRequestDataContract CreateDataContract()
        {
            return new PedestrianAuthenticationRequestDataContract
            {
                PhoneNumber = PhoneNumber,
                SkypeNumber = SkypeNumber
            };
        }
    }
}
