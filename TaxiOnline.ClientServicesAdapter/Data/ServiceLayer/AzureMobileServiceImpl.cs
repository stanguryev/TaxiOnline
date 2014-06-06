using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Services;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.ClientServicesAdapter.Data.DataObjects;
using TaxiOnline.ClientServicesAdapter.Data.Service;
using TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.ServiceLayerObjects;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer
{
    public class AzureMobileServiceImpl : IDataService
    {
        private readonly AzureMobileServicesProxy _proxy;

        public ClientInfrastructure.Data.ConnectionState ConnectionState
        {
            get { return ClientInfrastructure.Data.ConnectionState.Online; }
        }

        public event EventHandler ConnectionStateChanged;

        public event EventHandler<Toolkit.Events.ValueEventArgs<ClientInfrastructure.ServicesEntities.DataService.IPedestrianInfo>> PedestrianInfoChanged;

        public event EventHandler<Toolkit.Events.ValueEventArgs<ClientInfrastructure.ServicesEntities.DataService.IPedestrianRequest>> PedestrianRequestChanged;

        public event EventHandler<Toolkit.Events.ValueEventArgs<ClientInfrastructure.ServicesEntities.DataService.IDriverResponse>> DriverResponseChanged;

        public AzureMobileServiceImpl(string serverEndpointAddress)
        {
            _proxy = new AzureMobileServicesProxy(serverEndpointAddress);
        }

        public Toolkit.Events.ActionResult<IEnumerable<ClientInfrastructure.ServicesEntities.DataService.ICityInfo>> EnumerateCities(string userCultureName)
        {
            return RequestCollection<ICityInfo, CityDTO>(client => GetAsyncResult(() => client.InvokeApiAsync<IEnumerable<CityDTO>>(string.Format("Dictionary/EnumerateCities/{0}", userCultureName))), dto => new CitySLO(dto));
        }

        public Toolkit.Events.ActionResult<IEnumerable<ClientInfrastructure.ServicesEntities.DataService.IPersonInfo>> EnumerateAllPersons(Guid cityId)
        {
            return ActionResult<IEnumerable<IPersonInfo>>.GetValidResult(new IPersonInfo[0]);
        }

        public Toolkit.Events.ActionResult<IEnumerable<ClientInfrastructure.ServicesEntities.DataService.IPedestrianInfo>> EnumeratePedestrians(Guid cityId)
        {
            return RequestCollection<IPedestrianInfo, PedestrianDTO>(client => GetAsyncResult(() => client.GetTable<PedestrianDTO>().ToCollectionAsync()), dto => new PedestrianSLO(dto));
        }

        public Toolkit.Events.ActionResult<IEnumerable<ClientInfrastructure.ServicesEntities.DataService.IDriverInfo>> EnumerateDrivers(Guid cityId)
        {
            return RequestCollection<IDriverInfo, DriverDTO>(client => GetAsyncResult(() => client.GetTable<DriverDTO>().ToCollectionAsync()), dto => new DriverSLO(dto));
        }

        public Toolkit.Events.ActionResult<IEnumerable<ClientInfrastructure.ServicesEntities.DataService.IPedestrianRequest>> EnumeratePedestrianRequests(Guid cityId)
        {
            throw new NotImplementedException();
        }

        public Toolkit.Events.ActionResult<IEnumerable<ClientInfrastructure.ServicesEntities.DataService.IDriverResponse>> EnumerateDriverResponses(Guid cityId)
        {
            throw new NotImplementedException();
        }

        public ClientInfrastructure.ServicesEntities.DataService.IPedestrianRequest CreatePedestrianRequest(Guid pedestrianId, Guid driverId)
        {
            throw new NotImplementedException();
        }

        public Toolkit.Events.ActionResult PushPedestrianRequest(ClientInfrastructure.ServicesEntities.DataService.IPedestrianRequest requestSLO)
        {
            throw new NotImplementedException();
        }

        public Toolkit.Events.ActionResult RemovePedestrianRequest(Guid requestId)
        {
            throw new NotImplementedException();
        }

        public Toolkit.Events.ActionResult<ClientInfrastructure.ServicesEntities.DataService.IDriverResponse> ConfirmPedestrianRequest(Guid requestId)
        {
            throw new NotImplementedException();
        }

        public Toolkit.Events.ActionResult RejectPedestrianRequest(Guid requestId)
        {
            throw new NotImplementedException();
        }

        public Toolkit.Events.ActionResult RemoveDriverResponse(Guid responseId)
        {
            throw new NotImplementedException();
        }

        public Toolkit.Events.ActionResult StopRejectPedestrianRequest(Guid requestId)
        {
            throw new NotImplementedException();
        }

        public ClientInfrastructure.ServicesEntities.DataService.IAuthenticationRequest CreateAuthenticationRequest(ClientInfrastructure.Data.ParticipantTypes requestType, string deviceId, Guid cityId)
        {
            throw new NotImplementedException();
        }

        public Toolkit.Events.ActionResult<ClientInfrastructure.ServicesEntities.DataService.IPersonInfo> Authenticate(ClientInfrastructure.ServicesEntities.DataService.IAuthenticationRequest request)
        {
            throw new NotImplementedException();
        }

        private T GetAsyncResult<T>(Func<Task<T>> function)
        {
            Task<T> task = function();
            task.Wait();
            return task.Result;
        }

        private ActionResult<IEnumerable<TResult>> RequestCollection<TResult, TDataObject>(Func<MobileServiceClient, IEnumerable<TDataObject>> requestDelegate, Func<TDataObject, TResult> convertDelegate)
        {
            _proxy.WaitConnectionCompleted();
            MobileServiceClient channel = _proxy.Channel;
            ActionResult<IEnumerable<TDataObject>> remoteResult = _proxy.RunRequestSafe(() => requestDelegate(channel), channel);
            return remoteResult.IsValid ? ActionResult<IEnumerable<TResult>>.GetValidResult(remoteResult.Result.Select(d => convertDelegate(d)).ToArray()) : ActionResult<IEnumerable<TResult>>.GetErrorResult(remoteResult);
        }
    }
}
