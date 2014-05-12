using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;

namespace TaxiOnline.Server.Core.Objects
{
    internal class PedestrianRequestInfo : IPedestrianRequestInfo
    {
        private readonly Guid _id;
        private readonly Guid _driverId;
        private readonly Guid _pedestrianId;
        private string _comment;

        public Guid Id
        {
            get { return _id; }
        }

        public Guid DriverId
        {
            get { return _driverId; }
        }

        public Guid PedestrianId
        {
            get { return _pedestrianId; }
        }

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        public PedestrianRequestInfo(Guid id, Guid driverId, Guid pedestrianId)
        {
            _id = id;
            _driverId = driverId;
            _pedestrianId = pedestrianId;
        }
    }
}
