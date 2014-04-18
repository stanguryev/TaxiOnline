using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.ServiceContract
{
    public interface ITaxiOnlineCallback
    {
        void OnPedestrianInfoUpdated(PedestrianDataContract data);
        void OnPedestrianRequestDataContractUpdated(PedestrianRequestDataContract data);
    }
}
