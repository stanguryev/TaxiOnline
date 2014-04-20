using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public ActionResult<IEnumerable<IPedestrianInfo>> EnumeratePedestrians()
        {
            return RequestCollection<IPedestrianInfo, PedestrianDataContract>(channel => channel.EnumeratePedestrians(), data => new PedestrianSLO(data));
        }

        public ActionResult<IEnumerable<IPedestrianRequest>> EnumeratePedestrianRequests()
        {
            return RequestCollection<IPedestrianRequest, PedestrianRequestDataContract>(channel => channel.EnumeratePedestrianRequests(), data => new PedestrianRequestSLO(data));
        }

        public ActionResult<IEnumerable<IDriverResponse>> EnumerateDriverResponses()
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

        public ActionResult RemovePedestrianRequest(Guid pedestrianId)
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

        public ActionResult<IPedestrianInfo> AuthenticateAsPedestrian(string deviceId)
        {
            throw new NotImplementedException();
        }

        public ActionResult<IDriverInfo> AuthenticateAsDriver(string deviceId)
        {
            throw new NotImplementedException();
        }

        private ActionResult<IEnumerable<TResult>> RequestCollection<TResult, TDataContract>(Func<ITaxiOnlineService, IEnumerable<TDataContract>> requestDelegate, Func<TDataContract, TResult> convertDelegate)
        {
            _proxy.WaitConnectionCompleted();
            ITaxiOnlineService channel = _proxy.Channel;
            ActionResult<IEnumerable<TDataContract>> remoteResult = _proxy.RunRequestSafe(() => requestDelegate(channel), channel);
            return remoteResult.IsValid ? ActionResult<IEnumerable<TResult>>.GetValidResult(remoteResult.Result.Select(d => convertDelegate(d)).ToArray()) : ActionResult<IEnumerable<TResult>>.GetErrorResult(remoteResult);
        }
    }
}
