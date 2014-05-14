using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.Logic.Models
{
    public class PedestrianModel : PersonModel
    {
        private PedestrianRequestModel _currentRequest;

        public PedestrianRequestModel CurrentRequest
        {
            get { return _currentRequest; }
            internal set
            {
                if (_currentRequest != value)
                {
                    _currentRequest = value;
                    OnCurrentRequestChanged();
                }
            }
        }

        public event EventHandler CurrentRequestChanged;

        public event EventHandler MadeCall;

        internal PedestrianModel(IPedestrianInfo info)
            : base(info)
        {

        }

        internal void InvokeMadeCall()
        {
            OnMadeCall();
        }

        protected virtual void OnCurrentRequestChanged()
        {
            EventHandler handler = CurrentRequestChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnMadeCall()
        {
            EventHandler handler = MadeCall;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
