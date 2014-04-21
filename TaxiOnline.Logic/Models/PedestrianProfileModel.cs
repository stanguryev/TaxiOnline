using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using TaxiOnline.Logic.Helpers;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.CollectionsDecorators;

namespace TaxiOnline.Logic.Models
{
    public class PedestrianProfileModel : ProfileModel
    {
        private PedestrianProfileRequestModel _currentRequest;
        private PedestrianProfileRequestModel _pendingRequest;
        private readonly SimpleCollectionLoadDecorator<DriverModel> _drivers;

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

        public IEnumerable<DriverModel> Drivers
        {
            get { return _drivers.Items; }
        }

        public event EventHandler CurrentRequestChanged;

        public event EventHandler PendingRequestChanged;

        public event EventHandler DriversChanged
        {
            add { _drivers.ItemsChanged += value; }
            remove { _drivers.ItemsChanged -= value; }
        }

        public event NotifyCollectionChangedEventHandler DriversCollectionChanged
        {
            add { _drivers.ItemsCollectionChanged += value; }
            remove { _drivers.ItemsCollectionChanged -= value; }
        }

        internal Func<Logic.PedestrianProfileRequestLogic> InitRequestDelegate { get; set; }

        internal Func<ActionResult<IEnumerable<Logic.DriverLogic>>> EnumerateDriversDelegate { get; set; }

        internal PedestrianProfileModel()
        {
            _drivers = new SimpleCollectionLoadDecorator<DriverModel>(EnumerateDrivers);
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

        internal void ModifyDriversCollection(Action<IList<DriverModel>> modificationDelegate)
        {
            _drivers.ModifyCollection(modificationDelegate);
        }

        private ActionResult<IEnumerable<DriverModel>> EnumerateDrivers()
        {
            return UpdateHelper.EnumerateModels(EnumerateDriversDelegate, l => l.Model);
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
