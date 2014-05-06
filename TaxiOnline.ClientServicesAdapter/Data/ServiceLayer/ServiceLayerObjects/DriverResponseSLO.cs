using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.ServiceLayerObjects
{
    internal class DriverResponseSLO : IDriverResponse
    {
        private Guid _id;
        private Guid _driverId;
        private Guid _requestId;
        private DriverResponseState _state;

        public Guid Id
        {
            get { return _id; }
        }

        public Guid DriverId
        {
            get { return _driverId; }
        }

        public Guid RequestId
        {
            get { return _requestId; }
        }

        public DriverResponseState State
        {
            get { return _state; }
            set { _state = value; }
        }

        public DriverResponseSLO(DriverResponseDataContract dataContract)
        {
            _id = dataContract.Id;
            _driverId = dataContract.DriverId;
            _requestId = dataContract.RequestId;
            _state = dataContract.IsAccepted ? DriverResponseState.Accepted : DriverResponseState.Declined;
        }
    }
}
