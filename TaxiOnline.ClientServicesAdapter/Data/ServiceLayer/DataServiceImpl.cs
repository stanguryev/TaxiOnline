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
            return RequestCollection<IPersonInfo, PersonDataContract>(channel => channel.EnumerateAllPersons(cityId), data =>
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

        public IPedestrianRequest CreatePedestrianRequest(Guid pedestrianId)
        {
            return new PedestrianRequestSLO(pedestrianId);
        }

        public ActionResult PushPedestrianRequest(IPedestrianRequest requestSLO)
        {
            ITaxiOnlineService channel = _proxy.Channel;
            return _proxy.RunRequestSafe(() => channel.PushPedestrianRequest(((PedestrianRequestSLO)requestSLO).GetDataContract()), channel);
        }

        public ActionResult RemovePedestrianRequest(Guid requestId)
        {
            throw new NotImplementedException();
        }

        public ActionResult<IDriverResponse> ConfirmPedestrianRequest(Guid pedestrianRequestId, Guid driverId)
        {
            throw new NotImplementedException();
        }

        public ActionResult RemoveDriverResponse(Guid responseId)
        {
            throw new NotImplementedException();
        }

        public ActionResult<IPersonInfo> Authenticate(IAuthenticationRequest request)
        {
            throw new NotImplementedException();
            ITaxiOnlineService channel = _proxy.Channel;
            ActionResult<PersonDataContract> result = _proxy.RunRequestSafe(() => channel.Authenticate(((AuthenticationRequestSLO)request).CreateDataContract()), channel);
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
