using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.ServiceLayerObjects
{
    internal class PedestrianRequestSLO : IPedestrianRequest
    {
        private Guid _id;
        private Guid _pedestrianId;
        private Guid _driverId;
        private string _targetName;
        private MapPoint _targetLocation;
        private decimal _paymentAmount;
        private string _currency;
        private string _comment;
        private bool _isCanceled;

        public Guid Id
        {
            get { return _id; }
        }

        public Guid PedestrianId
        {
            get { return _pedestrianId; }
        }

        public Guid DriverId
        {
            get { return _driverId; }
        }
        
        public string TargetName
        {
            get { return _targetName; }
            set { _targetName = value; }
        }

        public MapPoint TargetLocation
        {
            get { return _targetLocation; }
            set { _targetLocation = value; }
        }

        public decimal PaymentAmount
        {
            get { return _paymentAmount; }
            set { _paymentAmount = value; }
        }

        public string Currency
        {
            get { return _currency; }
            set { _currency = value; }
        }

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        public bool IsCanceled
        {
            get { return _isCanceled; }
            set { _isCanceled = value; }
        }

        public PedestrianRequestSLO(PedestrianRequestDataContract dataContract)
        {
            _id = dataContract.Id;
            _driverId = dataContract.DriverId;
            _pedestrianId = dataContract.PedestrianId;
            _driverId = dataContract.DriverId;
            _comment = dataContract.Comment;
        }

        public PedestrianRequestSLO(Guid pedestrianId, Guid driverId)
        {
            _id = Guid.NewGuid();
            _pedestrianId = pedestrianId;
            _driverId = driverId;
        }

        public PedestrianRequestDataContract GetDataContract()
        {
            return new PedestrianRequestDataContract
            {
                Id = _id,
                DriverId = _driverId,
                PedestrianId = _pedestrianId,
                Comment = _comment,
                IsCanceled = _isCanceled
            };
        }
    }
}
