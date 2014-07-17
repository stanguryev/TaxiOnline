using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.ClientInfrastructure.Services
{
    public interface IDataService
    {
        ConnectionState ConnectionState { get; }
        event EventHandler ConnectionStateChanged;
        event EventHandler<ValueEventArgs<IDriverInfo>> DriverInfoChanged;
        event EventHandler<ValueEventArgs<IPedestrianInfo>> PedestrianInfoChanged;
        event EventHandler<ValueEventArgs<IPedestrianRequest>> PedestrianRequestChanged;
        event EventHandler<ValueEventArgs<IDriverResponse>> DriverResponseChanged;
        ActionResult<IEnumerable<ICityInfo>> EnumerateCities(string userCultureName);
        ActionResult<IEnumerable<IPersonInfo>> EnumerateAllPersons(Guid cityId);
        ActionResult<IEnumerable<IPedestrianInfo>> EnumeratePedestrians(Guid cityId);
        ActionResult<IEnumerable<IDriverInfo>> EnumerateDrivers(Guid cityId);
        ActionResult<IEnumerable<IPedestrianRequest>> EnumeratePedestrianRequests(Guid cityId);
        ActionResult<IEnumerable<IDriverResponse>> EnumerateDriverResponses(Guid cityId);
        IPedestrianRequest CreatePedestrianRequest(Guid pedestrianId, Guid driverId);
        ActionResult PushPedestrianRequest(IPedestrianRequest requestSLO);
        ActionResult RemovePedestrianRequest(Guid requestId);
        ActionResult<IDriverResponse> ConfirmPedestrianRequest(Guid requestId);
        ActionResult RejectPedestrianRequest(Guid requestId);
        ActionResult RemoveDriverResponse(Guid responseId);
        ActionResult StopRejectPedestrianRequest(Guid requestId);
        IAuthenticationRequest CreateAuthenticationRequest(ParticipantTypes requestType, string deviceId, Guid cityId);
        ActionResult<IPersonInfo> Authenticate(IAuthenticationRequest request);
    }
}
