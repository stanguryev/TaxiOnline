using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.Services;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.ClientServicesAdapter.Data.Service;
using TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.ServiceLayerObjects;
using TaxiOnline.ServiceContract;
using TaxiOnline.ServiceContract.DataContracts;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer
{
    public class DataServiceImpl : IDataService
    {
        private ServiceProxy _proxy;
        private CallbackManager _callbackManager = new CallbackManager();

        public event EventHandler<ValueEventArgs<IPedestrianInfo>> PedestrianInfoChanged
        {
            add { _proxy.CallbackWrapper.PedestrianInfoChangedDelegate += _callbackManager.GetContractUpdatedDelegate<PedestrianDataContract, IPedestrianInfo>(value, data => new PedestrianSLO(data)); }
            remove { _proxy.CallbackWrapper.PedestrianInfoChangedDelegate -= _callbackManager.GetContractUpdatedDelegate<PedestrianDataContract, IPedestrianInfo>(value, data => new PedestrianSLO(data)); }
        }

        public event EventHandler<ValueEventArgs<IPedestrianRequest>> PedestrianRequestChanged
        {
            add { _proxy.CallbackWrapper.PedestrianRequestChangedDelegate += _callbackManager.GetContractUpdatedDelegate<PedestrianRequestDataContract, IPedestrianRequest>(value, data => new PedestrianRequestSLO(data)); }
            remove { _proxy.CallbackWrapper.PedestrianRequestChangedDelegate -= _callbackManager.GetContractUpdatedDelegate<PedestrianRequestDataContract, IPedestrianRequest>(value, data => new PedestrianRequestSLO(data)); }
        }

        public event EventHandler<ValueEventArgs<IDriverResponse>> DriverResponseChanged;

        public DataServiceImpl(string serverEndpointAddress)
        {
            _proxy = new ServiceProxy(serverEndpointAddress);
        }

        public ActionResult<IEnumerable<ICityInfo>> EnumerateCities(string userCultureName)
        {
            return RequestCollection<ICityInfo, CityDataContract>(channel => channel.EnumerateCities(userCultureName), data => new CitySLO(data));
        }

        public ActionResult<IEnumerable<IPersonInfo>> EnumerateAllPersons(Guid cityId)
        {
            return RequestCollection<IPersonInfo, PersonDataContract>(channel => channel.EnumeratePedestrians(cityId).Union<PersonDataContract>(channel.EnumerateDrivers(cityId)), data =>
            {
                if (data is PedestrianDataContract)
                    return new PedestrianSLO((PedestrianDataContract)data);
                else if (data is DriverDataContract)
                    return new DriverSLO((DriverDataContract)data);
                else
                    throw new NotImplementedException();
            });
        }

        public ActionResult<IEnumerable<IPedestrianInfo>> EnumeratePedestrians(Guid cityId)
        {
            return RequestCollection<IPedestrianInfo, PedestrianDataContract>(channel => channel.EnumeratePedestrians(cityId), data => new PedestrianSLO(data));
        }

        public ActionResult<IEnumerable<IDriverInfo>> EnumerateDrivers(Guid cityId)
        {
            return RequestCollection<IDriverInfo, DriverDataContract>(channel => channel.EnumerateDrivers(cityId), data => new DriverSLO(data));
        }

        public ActionResult<IEnumerable<IPedestrianRequest>> EnumeratePedestrianRequests(Guid cityId)
        {
            return RequestCollection<IPedestrianRequest, PedestrianRequestDataContract>(channel => channel.EnumeratePedestrianRequests(cityId), data => new PedestrianRequestSLO(data));
        }

        public ActionResult<IEnumerable<IDriverResponse>> EnumerateDriverResponses(Guid cityId)
        {
            throw new NotImplementedException();
        }

        public IPedestrianRequest CreatePedestrianRequest(Guid pedestrianId, Guid driverId)
        {
            return new PedestrianRequestSLO(pedestrianId, driverId);
        }

        public ActionResult PushPedestrianRequest(IPedestrianRequest requestSLO)
        {
            ITaxiOnlineService channel = _proxy.Channel;
            return _proxy.RunRequestSafe(() => channel.PushPedestrianRequest(((PedestrianRequestSLO)requestSLO).GetDataContract()), channel);
        }

        public ActionResult RemovePedestrianRequest(Guid requestId)
        {
            ITaxiOnlineService channel = _proxy.Channel;
            return _proxy.RunRequestSafe(() => channel.RemovePedestrianRequest(requestId), channel);
        }

        public ActionResult<IDriverResponse> ConfirmPedestrianRequest(Guid requestId)
        {
            ITaxiOnlineService channel = _proxy.Channel;
            ActionResult<DriverResponseDataContract> confirmResult = _proxy.RunRequestSafe(() => channel.ConfirmPedestrianRequest(requestId), channel);
            return confirmResult.IsValid ? ActionResult<IDriverResponse>.GetValidResult(new DriverResponseSLO(confirmResult.Result)) : ActionResult<IDriverResponse>.GetErrorResult(confirmResult);
        }

        public ActionResult RemoveDriverResponse(Guid responseId)
        {
            ITaxiOnlineService channel = _proxy.Channel;
            return _proxy.RunRequestSafe(() => channel.RemoveDriverResponse(responseId), channel);
        }

        public ActionResult RejectPedestrianRequest(Guid requestId)
        {
            ITaxiOnlineService channel = _proxy.Channel;
            return _proxy.RunRequestSafe(() => channel.RejectPedestrianRequest(requestId), channel);
        }

        public ActionResult StopRejectPedestrianRequest(Guid requestId)
        {
            ITaxiOnlineService channel = _proxy.Channel;
            return _proxy.RunRequestSafe(() => channel.StopRejectPedestrianRequest(requestId), channel);
        }

        public ActionResult<IPersonInfo> Authenticate(IAuthenticationRequest request)
        {
            ITaxiOnlineService channel = _proxy.Channel;
            ActionResult<PersonDataContract> result = _proxy.RunRequestSafe(() =>
            {
                if (request is IPedestrianAuthenticationRequest)
                    return (PersonDataContract)channel.AuthenticateAsPedestrian((PedestrianAuthenticationRequestDataContract)((AuthenticationRequestSLO)request).CreateDataContract());
                else if (request is IDriverAuthenticationRequest)
                    return (PersonDataContract)channel.AuthenticateAsDriver((DriverAuthenticationRequestDataContract)((AuthenticationRequestSLO)request).CreateDataContract());
                else
                    throw new NotImplementedException();
            }, channel);
            return result.IsValid ? ActionResult<IPersonInfo>.GetValidResult(CreatePersonInfo(result.Result)) : ActionResult<IPersonInfo>.GetErrorResult(result);
        }

        private ActionResult<IEnumerable<TResult>> RequestCollection<TResult, TDataContract>(Func<ITaxiOnlineService, IEnumerable<TDataContract>> requestDelegate, Func<TDataContract, TResult> convertDelegate)
        {
            _proxy.WaitConnectionCompleted();
            ITaxiOnlineService channel = _proxy.Channel;
            ActionResult<IEnumerable<TDataContract>> remoteResult = _proxy.RunRequestSafe(() => requestDelegate(channel), channel);
            return remoteResult.IsValid ? ActionResult<IEnumerable<TResult>>.GetValidResult(remoteResult.Result.Select(d => convertDelegate(d)).ToArray()) : ActionResult<IEnumerable<TResult>>.GetErrorResult(remoteResult);
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

        private IPersonInfo CreatePersonInfo(PersonDataContract personDataContract)
        {
            DriverDataContract driverDataContract = personDataContract as DriverDataContract;
            if (driverDataContract != null)
                return new DriverSLO(driverDataContract);
            PedestrianDataContract pedestrianDataContract = personDataContract as PedestrianDataContract;
            if (pedestrianDataContract != null)
                return new PedestrianSLO(pedestrianDataContract);
            throw new NotImplementedException();
        }
    }
}
