using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.Logic.Common.Enums;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.Logic.Models
{
    public class DriverResponseModel
    {
        private Guid _responseId;
        private readonly DriverModel _responseAuthor;
        private readonly PedestrianProfileRequestModel _request;
        private DriverResponseState _state;

        public DriverResponseState State
        {
            get { return _state; }
            internal set
            {
                if (_state != value)
                {
                    _state = value;
                    OnStateChanged();
                }
            }
        }
        
        public Guid ResponseId
        {
            get { return _responseId; }
            internal set { _responseId = value; }
        }

        public DriverModel ResponseAuthor
        {
            get { return _responseAuthor; }
        } 

        public PedestrianProfileRequestModel Request
        {
            get { return _request; }
        }

        public event EventHandler StateChanged;

        public DriverResponseModel(PedestrianProfileRequestModel request, DriverModel responseAuthor)
        {
            _responseAuthor = responseAuthor;
            _request = request;
        }

        public string GetBriefInfo()
        {
            return _responseAuthor.CarBrand;
        }

        protected virtual void OnStateChanged()
        {
            EventHandler handler = StateChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
