using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.ServiceContract;
using TaxiOnline.ServiceContract.DataContracts;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.ClientServicesAdapter.Data.Service
{
    internal class CallbackWrapper : ITaxiOnlineCallback
    {
        public Action<PedestrianDataContract> PedestrianInfoChangedDelegate;

        public Action<PedestrianRequestDataContract> PedestrianRequestChangedDelegate;

        public void OnPedestrianInfoUpdated(PedestrianDataContract data)
        {
            Action<PedestrianDataContract> handler = PedestrianInfoChangedDelegate;
            if (handler != null)
                handler(data);
        }

        public void OnPedestrianRequestDataContractUpdated(PedestrianRequestDataContract data)
        {
            Action<PedestrianRequestDataContract> handler = PedestrianRequestChangedDelegate;
            if (handler != null)
                handler(data);
        }
    }
}
