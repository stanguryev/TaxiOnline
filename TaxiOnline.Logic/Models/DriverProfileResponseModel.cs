using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.Logic.Common.Enums;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.Logic.Models
{
    public class DriverProfileResponseModel
    {
        private Guid _responseId;
        private DriverProfileModel _responseAuthor;
        private PedestrianRequestModel _request;
        private RequestState _state;

        public Guid ResponseId
        {
            get { return _responseId; }
            internal set { _responseId = value; }
        }

        public RequestState State
        {
            get { return _state; }
            internal set { _state = value; }
        }

        public event ActionResultEventHandler ConfirmApplied;

        public event ActionResultEventHandler CancelApplied;

        internal Action<Action<ActionResult>> ConfirmDelegate { get; set; }

        internal Action<Action<ActionResult>> CancelDelegate { get; set; }

        internal DriverProfileResponseModel(PedestrianRequestModel request, DriverProfileModel responseAuthor)
        {
            _responseAuthor = responseAuthor;
            _request = request;
        }

        public void Confirm()
        {
            Action<Action<ActionResult>> confirmDelegate = ConfirmDelegate;
            if (confirmDelegate != null)
                confirmDelegate(OnConfirmApplied);
        }

        public void Cancel()
        {
            Action<Action<ActionResult>> cancelDelegate = CancelDelegate;
            if (cancelDelegate != null)
                cancelDelegate(OnCancelApplied);
        }

        protected virtual void OnConfirmApplied(ActionResult result)
        {
            ActionResultEventHandler handler = ConfirmApplied;
            if (handler != null)
                handler(this, new ActionResultEventArgs(result));
        }

        protected virtual void OnCancelApplied(ActionResult result)
        {
            ActionResultEventHandler handler = CancelApplied;
            if (handler != null)
                handler(this, new ActionResultEventArgs(result));
        }
    }
}
