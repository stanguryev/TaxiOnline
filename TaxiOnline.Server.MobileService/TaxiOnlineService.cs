using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ServerInfrastructure;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;
using TaxiOnline.ServiceContract;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.Server.MobileService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [ServiceKnownType(typeof(DriverAuthenticationRequestDataContract))]
    [ServiceKnownType(typeof(PedestrianAuthenticationRequestDataContract))]
    internal class TaxiOnlineService : ITaxiOnlineService
    {
        private readonly ITaxiOnlineServer _server;

        public TaxiOnlineService(ITaxiOnlineServer server)
        {
            _server = server;
        }

        public IEnumerable<CityDataContract> EnumerateCities(string userCultureName)
        {
            return _server.EnumerateCities(userCultureName).Select(city => ConvertHelper.CreateCityDataContract(city)).ToArray();
        }

        public PedestrianDataContract AuthenticateAsPedestrian(PedestrianAuthenticationRequestDataContract request)
        {
            IPedestrianInfo pedestrianInfo = _server.CreatePedestrianInfo();
            ConvertHelper.FillPedestrianAuthenticationRequestInfo(pedestrianInfo, request);
            _server.ModifyPedestriansCollection(col => col.Add(pedestrianInfo));
            return ConvertHelper.CreatePedestrianDataContract(pedestrianInfo);
        }

        public DriverDataContract AuthenticateAsDriver(DriverAuthenticationRequestDataContract request)
        {
            IDriverInfo driverInfo = _server.CreateDriverInfo();
            ConvertHelper.FillDriverAuthenticationRequestInfo(driverInfo, request);
            _server.ModifyDriversCollection(col => col.Add(driverInfo));
            return ConvertHelper.CreateDriverDataContract(driverInfo);
        }

        public IEnumerable<PedestrianDataContract> EnumeratePedestrians(Guid cityId)
        {
            return _server.Pedestrians.Select(p => ConvertHelper.CreatePedestrianDataContract(p)).ToArray();
        }

        public IEnumerable<DriverDataContract> EnumerateDrivers(Guid cityId)
        {
            return _server.Drivers.Select(d => ConvertHelper.CreateDriverDataContract(d)).ToArray();
        }

        public IEnumerable<PedestrianRequestDataContract> EnumeratePedestrianRequests(Guid cityId)
        {
            throw new NotImplementedException();
        }

        public void PushPedestrianRequest(PedestrianRequestDataContract request)
        {
            throw new NotImplementedException();
        }
        
        public void RemovePedestrianRequest(Guid requestId)
        {
            throw new NotImplementedException();
        }

        public DriverResponseDataContract ConfirmPedestrianRequest(Guid pedestrianRequestId)
        {
            throw new NotImplementedException();
        }

        public void RemoveDriverResponse(Guid responseId)
        {
            throw new NotImplementedException();
        }

        public void RejectPedestrianRequest(Guid requestId)
        {
            throw new NotImplementedException();
        }

        public void StopRejectPedestrianRequest(Guid requestId)
        {
            throw new NotImplementedException();
        }
    }
}
