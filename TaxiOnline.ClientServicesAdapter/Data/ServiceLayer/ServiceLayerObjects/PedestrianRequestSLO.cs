using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.ServiceLayerObjects
{
    internal class PedestrianRequestSLO : IPedestrianRequest
    {
        public Guid Id
        {
            get { throw new NotImplementedException(); }
        }

        public Guid PedestrianId
        {
            get { throw new NotImplementedException(); }
        }

        public string TargetName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ClientInfrastructure.Data.MapPoint TargetLocation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public decimal PaymentAmount
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Currency
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsCanceled
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public PedestrianRequestSLO(PedestrianRequestDataContract dataContract)
        {
           
        }

        public PedestrianRequestSLO(Guid pedestrianId)
        {
            
        }

        public PedestrianRequestDataContract GetDataContract()
        {
            return new PedestrianRequestDataContract
            {

            };
        }
    }
}
