using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TaxiOnline.Logic.Common.Enums;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.CollectionsDecorators;

namespace TaxiOnline.Logic.Models
{
    public class PedestrianProfileRequestModel
    {
        private readonly PedestrianProfileModel _user;
        private Guid _requestId;
        private decimal _paymentAmount;
        private string _currency = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
        private MapLocationModel _target = new MapLocationModel();
        private readonly ReadonlyCollectionDecorator<DriverResponseModel> _availableResponses = new ReadonlyCollectionDecorator<DriverResponseModel>();
        private DriverResponseModel _acceptedResponse;
        private RequestState _state;

        public Guid RequestId
        {
            get { return _requestId; }
            internal set { _requestId = value; }
        }

        public RequestState State
        {
            get { return _state; }
            internal set { _state = value; }
        }

        public MapLocationModel Target
        {
            get { return _target; }
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

        public event ActionResultEventHandler ConfirmApplied;

        public event ActionResultEventHandler CancelApplied;

        internal Action<Action<ActionResult>> ConfirmDelegate { get; set; }

        internal Action<Action<ActionResult>> CancelDelegate { get; set; }

        internal PedestrianProfileRequestModel(PedestrianProfileModel user)
        {
            _user = user;
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
