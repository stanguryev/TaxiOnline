using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using TaxiOnline.Logic.Common.Enums;
using TaxiOnline.Logic.Helpers;
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
        private readonly SimpleCollectionLoadDecorator<DriverResponseModel> _availableResponses;
        private DriverResponseModel _selectedResponse;
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

        public IEnumerable<DriverResponseModel> AvailableResponses
        {
            get { return _availableResponses.Items; }
        }

        public DriverResponseModel SelectedResponse
        {
            get { return _selectedResponse; }
            set
            {
                if (_selectedResponse != value)
                {
                    _selectedResponse = value;
                    OnSelectedResponseChanged();
                }
            }
        }

        public event ActionResultEventHandler ConfirmApplied;

        public event ActionResultEventHandler CancelApplied;

        public event EventHandler SelectedResponseChanged;

        public event EventHandler AvailableResponsesChanged
        {
            add { _availableResponses.ItemsChanged += value; }
            remove { _availableResponses.ItemsChanged -= value; }
        }

        public event NotifyCollectionChangedEventHandler AvailableResponsesCollectionChanged
        {
            add { _availableResponses.ItemsCollectionChanged += value; }
            remove { _availableResponses.ItemsCollectionChanged -= value; }
        }

        internal Action<Action<ActionResult>> ConfirmDelegate { get; set; }

        internal Action<Action<ActionResult>> CancelDelegate { get; set; }

        internal Func<ActionResult> CallToDriverDelegate { get; set; }

        internal Func<ActionResult<IEnumerable<Logic.DriverResponseLogic>>> EnumerateAvailableResponsesDelegate;

        internal PedestrianProfileRequestModel(PedestrianProfileModel user)
        {
            _user = user;
            _availableResponses = new SimpleCollectionLoadDecorator<DriverResponseModel>(EnumerateAvailableResponses);
        }

        private ActionResult<IEnumerable<DriverResponseModel>> EnumerateAvailableResponses()
        {
            return UpdateHelper.EnumerateModels(EnumerateAvailableResponsesDelegate, l => l.Model);
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

        public ActionResult CallToDriver()
        {
            Func<ActionResult> callToDriverDelegate = CallToDriverDelegate;
            if (callToDriverDelegate != null)
                return callToDriverDelegate();
            return ActionResult.GetErrorResult(new NotSupportedException());
        }

        internal void ModifyDriverResponsesCollection(Action<IList<DriverResponseModel>> modificationDelegate)
        {
            _availableResponses.ModifyCollection(modificationDelegate);
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

        protected virtual void OnSelectedResponseChanged()
        {
            EventHandler handler = SelectedResponseChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
