using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.ClientInfrastructure.Services
{
    public interface IDataService
    {
        event EventHandler<ValueEventArgs<IPedestrianInfo>> PedestrianInfoChanged;
        event EventHandler<ValueEventArgs<IPedestrianRequest>> PedestrianRequestChanged;
        event EventHandler<ValueEventArgs<IDriverResponse>> DriverResponseChanged;
        ActionResult<IEnumerable<ICityInfo>> EnumerateCities(string userCultureName);
        ActionResult<IEnumerable<IPersonInfo>> EnumerateAllPersons(Guid cityId);
        ActionResult<IEnumerable<IPedestrianInfo>> EnumeratePedestrians(Guid cityId);
        ActionResult<IEnumerable<IPedestrianRequest>> EnumeratePedestrianRequests(Guid cityId);
        ActionResult<IEnumerable<IDriverResponse>> EnumerateDriverResponses(Guid cityId);
        IPedestrianRequest CreatePedestrianRequest(Guid pedestrianId);
        ActionResult PushPedestrianRequest(IPedestrianRequest requestSLO);
        ActionResult RemovePedestrianRequest(Guid requestId);
        ActionResult<IDriverResponse> ConfirmPedestrianRequest(Guid pedestrianRequestId, Guid driverId);
        ActionResult RemoveDriverResponse(Guid responseId);
        ActionResult<IPedestrianInfo> AuthenticateAsPedestrian(string deviceId);
        ActionResult<IDriverInfo> AuthenticateAsDriver(string deviceId);
    }
}
