using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.Services;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.ClientServicesAdapter.Data.DataObjects;
using TaxiOnline.ClientServicesAdapter.Data.Service;
using TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.Decorators;
using TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.ServiceLayerObjects;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.Patterns;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer
{
    public class AzureMobileServiceImpl : IDataService
    {
        private readonly AzureMobileServicesProxy _proxy;
        private CollectionTraceDecorator<DriverDTO, IDriverInfo> _driversTracker;

        public ConnectionState ConnectionState
        {
            get { return ConnectionState.Online; }
        }

        public event EventHandler ConnectionStateChanged;

        public event EventHandler<ValueEventArgs<IDriverInfo>> DriverInfoChanged;

        public event EventHandler<Toolkit.Events.ValueEventArgs<ClientInfrastructure.ServicesEntities.DataService.IPedestrianInfo>> PedestrianInfoChanged;

        public event EventHandler<Toolkit.Events.ValueEventArgs<ClientInfrastructure.ServicesEntities.DataService.IPedestrianRequest>> PedestrianRequestChanged;

        public event EventHandler<Toolkit.Events.ValueEventArgs<ClientInfrastructure.ServicesEntities.DataService.IDriverResponse>> DriverResponseChanged;

        public AzureMobileServiceImpl(string serverEndpointAddress)
        {
            _proxy = new AzureMobileServicesProxy(serverEndpointAddress);
        }

        public ActionResult<IEnumerable<ICityInfo>> EnumerateCities(string userCultureName)
        {
            return RequestCollection<ICityInfo, CityDTO>(client => GetAsyncResult(() => client.InvokeApiAsync<IEnumerable<CityDTO>>(string.Format("Dictionary/EnumerateCities/{0}", userCultureName))), dto => new CitySLO(dto));
        }

        public ActionResult<IEnumerable<IPersonInfo>> EnumerateAllPersons(Guid cityId)
        {
            return ActionResult<IEnumerable<IPersonInfo>>.GetValidResult(new IPersonInfo[0]);
        }

        public ActionResult<IEnumerable<IPedestrianInfo>> EnumeratePedestrians(Guid cityId)
        {
            return RequestCollection<IPedestrianInfo, PedestrianDTO>(client => GetAsyncResult(() => client.GetTable<PedestrianDTO>().Where(dto => dto.CityId == cityId).ToListAsync()), dto => new PedestrianSLO(dto));
        }

        public ActionResult<IEnumerable<IDriverInfo>> EnumerateDrivers(Guid cityId)
        {
            if (_driversTracker != null)
                _driversTracker.GotUpdate -= DriversTracker_GotUpdate;
            _driversTracker = new CollectionTraceDecorator<DriverDTO, IDriverInfo>(_proxy, client => client.GetTable<DriverDTO>(), dto => dto.CityId == cityId, dto => new DriverSLO(dto));
            _driversTracker.GotUpdate += DriversTracker_GotUpdate;
            return _driversTracker.Init();
            //return RequestCollection<IDriverInfo, DriverDTO>(client => GetAsyncResult(() => client.GetTable<DriverDTO>().Where(dto => dto.CityId == cityId).ToListAsync()), dto => new DriverSLO(dto));
        }

        public ActionResult<IEnumerable<IPedestrianRequest>> EnumeratePedestrianRequests(Guid cityId)
        {
            return ActionResult<IEnumerable<IPedestrianRequest>>.GetValidResult(new IPedestrianRequest[0]);
        }

        public ActionResult<IEnumerable<IDriverResponse>> EnumerateDriverResponses(Guid cityId)
        {
            return ActionResult<IEnumerable<IDriverResponse>>.GetValidResult(new IDriverResponse[0]);
        }

        public IPedestrianRequest CreatePedestrianRequest(Guid pedestrianId, Guid driverId)
        {
            return new PedestrianRequestSLO(pedestrianId, driverId);
        }

        public ActionResult PushPedestrianRequest(IPedestrianRequest requestSLO)
        {
            throw new NotImplementedException();
        }

        public ActionResult RemovePedestrianRequest(Guid requestId)
        {
            throw new NotImplementedException();
        }

        public ActionResult<IDriverResponse> ConfirmPedestrianRequest(Guid requestId)
        {
            throw new NotImplementedException();
        }

        public ActionResult RejectPedestrianRequest(Guid requestId)
        {
            throw new NotImplementedException();
        }

        public ActionResult RemoveDriverResponse(Guid responseId)
        {
            throw new NotImplementedException();
        }

        public ActionResult StopRejectPedestrianRequest(Guid requestId)
        {
            throw new NotImplementedException();
        }

        public IAuthenticationRequest CreateAuthenticationRequest(ParticipantTypes requestType, string deviceId, Guid cityId)
        {
            switch (requestType)
            {
                case ParticipantTypes.Driver:
                    return new DriverAuthenticationRequestSLO(cityId);
                    break;
                case ParticipantTypes.Pedestrian:
                    return new PedestrianAuthenticationRequestSLO(cityId);
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        public ActionResult<IPersonInfo> Authenticate(IAuthenticationRequest request)
        {
            MobileServiceClient client = _proxy.Channel;
            if (request is IPedestrianAuthenticationRequest)
            {
                ActionResult<PedestrianDTO> dtoResult = _proxy.RunRequestSafe(() => GetAsyncResult(() => client.InvokeApiAsync<PedestrianAuthenticationDTO, PedestrianDTO>("Authentication/AuthenticateAsPedestrian", ((PedestrianAuthenticationRequestSLO)request).CreateDataObject(), System.Net.Http.HttpMethod.Post, new Dictionary<string, string>())), client);
                return dtoResult.IsValid ? ActionResult<IPersonInfo>.GetValidResult(new PedestrianSLO(dtoResult.Result)) : ActionResult<IPersonInfo>.GetErrorResult(dtoResult); ;
            }
            if (request is IDriverAuthenticationRequest)
            {
                ActionResult<DriverDTO> dtoResult = _proxy.RunRequestSafe(() => GetAsyncResult(() => client.InvokeApiAsync<DriverAuthenticationDTO, DriverDTO>("Authentication/AuthenticateAsDriver", ((DriverAuthenticationRequestSLO)request).CreateDataObject(), System.Net.Http.HttpMethod.Post, new Dictionary<string, string>())), client);
                return dtoResult.IsValid ? ActionResult<IPersonInfo>.GetValidResult(new DriverSLO(dtoResult.Result)) : ActionResult<IPersonInfo>.GetErrorResult(dtoResult); ;
            }
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

        protected virtual void OnDriverInfoChanged(IDriverInfo driverInfo)
        {
            EventHandler<ValueEventArgs<IDriverInfo>> handler = DriverInfoChanged;
            if (handler != null)
                handler(this, new ValueEventArgs<IDriverInfo>(driverInfo));
        }

        private void DriversTracker_GotUpdate(object sender, ValueEventArgs<IDriverInfo> e)
        {
            OnDriverInfoChanged(e.Value);
        }
    }
}
