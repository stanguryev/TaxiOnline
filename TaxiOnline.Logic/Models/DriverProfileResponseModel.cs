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
        private RequestState _confirmState;
        private RequestState _rejectState;

        public Guid ResponseId
        {
            get { return _responseId; }
            internal set { _responseId = value; }
        }

        public RequestState ConfirmState
        {
            get { return _confirmState; }
            internal set { _confirmState = value; }
        }

        public RequestState RejectState
        {
            get { return _rejectState; }
            internal set { _rejectState = value; }
        }

        public event ActionResultEventHandler ConfirmApplied;

        public event ActionResultEventHandler CancelConfirmApplied;

        public event ActionResultEventHandler RejectApplied;

        public event ActionResultEventHandler CancelRejectApplied;

        internal Action<Action<ActionResult>> ConfirmDelegate { get; set; }

        internal Action<Action<ActionResult>> CancelConfirmDelegate { get; set; }

        internal Action<Action<ActionResult>> RejectDelegate { get; set; }

        internal Action<Action<ActionResult>> CancelRejectDelegate { get; set; }

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

        public void CancelConfirm()
        {
            Action<Action<ActionResult>> cancelConfirmDelegate = CancelConfirmDelegate;
            if (cancelConfirmDelegate != null)
                cancelConfirmDelegate(OnCancelConfirmApplied);
        }

        public void Reject()
        {
            Action<Action<ActionResult>> rejectDelegate = RejectDelegate;
            if (rejectDelegate != null)
                rejectDelegate(OnRejectApplied);
        }

        public void CancelReject()
        {
            Action<Action<ActionResult>> cancelRejectDelegate = CancelRejectDelegate;
            if (cancelRejectDelegate != null)
                cancelRejectDelegate(OnCancelRejectApplied);
        }

        protected virtual void OnConfirmApplied(ActionResult result)
        {
            ActionResultEventHandler handler = ConfirmApplied;
            if (handler != null)
                handler(this, new ActionResultEventArgs(result));
        }

        protected virtual void OnCancelConfirmApplied(ActionResult result)
        {
            ActionResultEventHandler handler = CancelConfirmApplied;
            if (handler != null)
                handler(this, new ActionResultEventArgs(result));
        }

        protected virtual void OnRejectApplied(ActionResult result)
        {
            ActionResultEventHandler handler = RejectApplied;
            if (handler != null)
                handler(this, new ActionResultEventArgs(result));
        }

        protected virtual void OnCancelRejectApplied(ActionResult result)
        {
            ActionResultEventHandler handler = CancelRejectApplied;
            if (handler != null)
                handler(this, new ActionResultEventArgs(result));
        }
    }
}
