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

        internal PedestrianModel(IPedestrianInfo info)
            : base(info)
        {

        }

        protected virtual void OnCurrentRequestChanged()
        {
            EventHandler handler = CurrentRequestChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
