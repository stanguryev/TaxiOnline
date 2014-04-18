using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.Logic.Models
{
    public class PedestrianProfileModel : ProfileModel
    {
        private PedestrianProfileRequestModel _currentRequest;
        private PedestrianProfileRequestModel _pendingRequest;

        public PedestrianProfileRequestModel CurrentRequest
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

        public PedestrianProfileRequestModel PendingRequest
        {
            get { return _pendingRequest; }
            internal set
            {
                if (_pendingRequest != value)
                {
                    _pendingRequest = value;
                    OnPendingRequestChanged();
                }
            }
        }

        internal Func<Logic.PedestrianProfileRequestLogic> InitRequestDelegate { get; set; }

        public event EventHandler CurrentRequestChanged;

        public event EventHandler PendingRequestChanged;

        internal PedestrianProfileModel()
        {

        }

        public ActionResult<PedestrianProfileRequestModel> InitRequest()
        {
            Func<Logic.PedestrianProfileRequestLogic> initRequestDelegate = InitRequestDelegate;
            if (initRequestDelegate != null)
            {
                Logic.PedestrianProfileRequestLogic logic = initRequestDelegate();
                return ActionResult<PedestrianProfileRequestModel>.GetValidResult(logic.Model);
            }
            else
                return ActionResult<PedestrianProfileRequestModel>.GetErrorResult(new NotSupportedException());
        }

        protected virtual void OnCurrentRequestChanged()
        {
            EventHandler handler = CurrentRequestChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnPendingRequestChanged()
        {
            EventHandler handler = PendingRequestChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
