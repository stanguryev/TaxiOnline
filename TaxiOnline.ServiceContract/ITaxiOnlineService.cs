using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.ServiceContract
{
    [System.ServiceModel.ServiceContract()]//CallbackContract = typeof(ITaxiOnlineCallback))]
    public interface ITaxiOnlineService
    {
        [OperationContract]
        IEnumerable<CityDataContract> EnumerateCities(string userCultureName);

        [OperationContract]
        IEnumerable<PersonDataContract> EnumerateAllPersons(Guid cityId);

        [OperationContract]
        PersonDataContract Authenticate(AuthenticationRequestDataContract request);

        [OperationContract]
        IEnumerable<PedestrianDataContract> EnumeratePedestrians(Guid cityId);

        [OperationContract]
        IEnumerable<DriverDataContract> EnumerateDrivers(Guid cityId);

        [OperationContract]
        IEnumerable<PedestrianRequestDataContract> EnumeratePedestrianRequests(Guid cityId);

        [OperationContract]
        void PushPedestrianRequest(PedestrianRequestDataContract request);
    }
}
