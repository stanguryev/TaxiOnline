using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.ServiceLayerObjects
{
    internal class DriverResponseSLO : IDriverResponse
    {
        private Guid _id;
        private Guid _driverId;
        private Guid _requestId;
        private bool _isCanceled;

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
        
        public bool IsCanceled
        {
            get { return _isCanceled; }
            set { _isCanceled = value; }
        }

        public DriverResponseSLO(DriverResponseDataContract dataContract)
        {
            _id = dataContract.Id;
            _driverId = dataContract.DriverId;
            _requestId = dataContract.RequestId;
            _isCanceled = dataContract.IsCanceled;
        }
    }
}
