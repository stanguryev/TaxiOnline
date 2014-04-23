using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.Logic.Common.Enums;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.Logic.Models
{
    public class DriverResponseModel
    {
        private Guid _responseId;
        private DriverModel _responseAuthor;
        private PedestrianProfileRequestModel _request;

        public Guid ResponseId
        {
            get { return _responseId; }
            internal set { _responseId = value; }
        }

        public DriverResponseModel(PedestrianProfileRequestModel request, DriverModel responseAuthor)
        {
            _responseAuthor = responseAuthor;
            _request = request;
        }

        
    }
}
