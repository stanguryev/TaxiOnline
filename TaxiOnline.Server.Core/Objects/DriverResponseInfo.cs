using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;

namespace TaxiOnline.Server.Core.Objects
{
    internal class DriverResponseInfo : IDriverResponseInfo
    {
        private readonly Guid _id;
        private readonly Guid _requestId;
        private bool _isAccepted;

        public Guid Id
        {
            get { return _id; }
        }

        public Guid RequestId
        {
            get { return _requestId; }
        }

        public bool IsAccepted
        {
            get { return _isAccepted; }
            set { _isAccepted = value; }
        }

        public DriverResponseInfo(Guid id, Guid requestId)
        {
            _id = id;
            _requestId = requestId;
        }
    }
}
