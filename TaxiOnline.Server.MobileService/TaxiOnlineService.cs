using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ServerInfrastructure;
using TaxiOnline.ServiceContract;

namespace TaxiOnline.Server.MobileService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    internal class TaxiOnlineService : ITaxiOnlineService
    {
        private readonly ITaxiOnlineServer _server;

        public TaxiOnlineService(ITaxiOnlineServer server)
        {
            _server = server;
        }

        public IEnumerable<ServiceContract.DataContracts.CityDataContract> EnumerateCities(string userCultureName)
        {
            return _server.EnumerateCities(userCultureName).Select(city => ConvertHelper.CreateCityDataContract(city)).ToArray();
        }

        public IEnumerable<ServiceContract.DataContracts.PersonDataContract> EnumerateAllPersons(Guid cityId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ServiceContract.DataContracts.PedestrianDataContract> EnumeratePedestrians(Guid cityId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ServiceContract.DataContracts.PedestrianRequestDataContract> EnumeratePedestrianRequests(Guid cityId)
        {
            throw new NotImplementedException();
        }

        public void PushPedestrianRequest(ServiceContract.DataContracts.PedestrianRequestDataContract request)
        {
            throw new NotImplementedException();
        }
    }
}
