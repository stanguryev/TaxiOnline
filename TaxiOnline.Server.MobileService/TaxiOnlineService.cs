using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ServerInfrastructure;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;
using TaxiOnline.ServerInfrastructure.LogicInterfaces;
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
            _server.AuthenticateAsPedestrian(pedestrianInfo, request.CityId);
            return ConvertHelper.CreatePedestrianDataContract(pedestrianInfo);
        }

        public DriverDataContract AuthenticateAsDriver(DriverAuthenticationRequestDataContract request)
        {
            IDriverInfo driverInfo = _server.CreateDriverInfo();
            ConvertHelper.FillDriverAuthenticationRequestInfo(driverInfo, request);
            _server.AuthenticateAsDriver(driverInfo, request.CityId);
            return ConvertHelper.CreateDriverDataContract(driverInfo);
        }

        public IEnumerable<PedestrianDataContract> EnumeratePedestrians(Guid cityId)
        {
            ICityLogic city = _server.Cities.FirstOrDefault(c => c.Info.Id == cityId);
            return cityId == null ? new PedestrianDataContract[0] : city.Pedestrians.Select(p => ConvertHelper.CreatePedestrianDataContract(p)).ToArray();
        }

        public IEnumerable<DriverDataContract> EnumerateDrivers(Guid cityId)
        {
            ICityLogic city = _server.Cities.FirstOrDefault(c => c.Info.Id == cityId);
            return cityId == null ? new DriverDataContract[0] : city.Drivers.Select(d => ConvertHelper.CreateDriverDataContract(d)).ToArray();
        }

        public IEnumerable<PedestrianRequestDataContract> EnumeratePedestrianRequests(Guid cityId)
        {
            ICityLogic city = _server.Cities.FirstOrDefault(c => c.Info.Id == cityId);
            return cityId == null ? new PedestrianRequestDataContract[0] : city.PedestrianRequests.Select(r => ConvertHelper.CreatePedestrianRequestsDataContract(r)).ToArray();
        }

        public void PushPedestrianRequest(PedestrianRequestDataContract request)
        {
            IPedestrianInfo pedestrian = _server.Cities.SelectMany(c => c.Pedestrians).FirstOrDefault(p => p.Id == request.PedestrianId);
            IDriverInfo driver = _server.Cities.SelectMany(c => c.Drivers).FirstOrDefault(d => d.Id == request.DriverId);
            if (pedestrian != null && driver != null)
            {
                IPedestrianRequestsInfo requestInfo = _server.CreatePedestrianRequestInfo(pedestrian, driver);
                ConvertHelper.FillPedestrianRequestInfo(requestInfo, request);
                _server.PushPedestrianRequestInfo(requestInfo);
            }
        }

        public void RemovePedestrianRequest(Guid requestId)
        {
            ICityLogic city = _server.Cities.FirstOrDefault(c => c.PedestrianRequests.Any(r => r.Id == requestId));
            if (city != null)
            {
                IPedestrianRequestsInfo request = city.PedestrianRequests.FirstOrDefault(r => r.Id == requestId);
                if (request != null)
                    city.ModifyPedestrianRequestsCollection(col => col.Remove(request));
            }
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
