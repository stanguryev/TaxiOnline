using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.ServiceContract
{
    [ServiceContract]//CallbackContract = typeof(ITaxiOnlineCallback))]
    public interface ITaxiOnlineService //: IDisposable
    {
        [OperationContract]
        IEnumerable<CityDataContract> EnumerateCities(string userCultureName);

        [OperationContract]
        PedestrianDataContract AuthenticateAsPedestrian(PedestrianAuthenticationRequestDataContract request);

        [OperationContract]
        DriverDataContract AuthenticateAsDriver(DriverAuthenticationRequestDataContract request);

        [OperationContract]
        IEnumerable<PedestrianDataContract> EnumeratePedestrians(Guid cityId);

        [OperationContract]
        IEnumerable<DriverDataContract> EnumerateDrivers(Guid cityId);

        [OperationContract]
        IEnumerable<PedestrianRequestDataContract> EnumeratePedestrianRequests(Guid cityId);

        [OperationContract]
        void PushPedestrianRequest(PedestrianRequestDataContract request);

        [OperationContract]
        DriverResponseDataContract ConfirmPedestrianRequest(Guid pedestrianRequestId);
    }
}
