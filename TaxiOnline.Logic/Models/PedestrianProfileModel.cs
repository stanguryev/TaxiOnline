using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private DriverModel _selectedDriver;

        public DriverModel SelectedDriver
        {
            get { return _selectedDriver; }
            set
            {
                if (_selectedDriver != value)
                {
                    _selectedDriver = value;
                    OnSelectedDriverChanged();
                }
            }
        }

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

        public event EventHandler LoadCompleted;

        public event EventHandler CheckCurrentRequestFailed;

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

        public event ActionResultEventHandler EnumrateDriversFailed
        {
            add { _drivers.RequestFailed += value; }
            remove { _drivers.RequestFailed -= value; }
        }

        public event EventHandler SelectedDriverChanged;

        internal Func<DriverModel, Logic.PedestrianProfileRequestLogic> InitRequestDelegate { get; set; }

        internal Func<ActionResult<Logic.PedestrianProfileRequestLogic>> CheckCurrentRequest { get; set; }

        internal Func<ActionResult<IEnumerable<Logic.DriverLogic>>> EnumerateDriversDelegate { get; set; }

        internal Func<DriverModel, ActionResult> CallToDriverDelegate { get; set; }

        internal PedestrianProfileModel(MapModel map)
            : base(map)
        {
            _drivers = new SimpleCollectionLoadDecorator<DriverModel>(EnumerateDrivers);
        }

        public void BeginLoad()
        {
            Task.Factory.StartNew(() =>
            {
                _drivers.FillItemsList();

                OnLoadCompleted();
                return;

                ActionResult<PedestrianProfileRequestModel> currentRequestResult = UpdateHelper.GetModel(CheckCurrentRequest, l => l.Model);
                if (currentRequestResult.IsValid)
                    CurrentRequest = currentRequestResult.Result;
                else
                {
                    OnCheckCurrentRequestFailed();
                    return;
                }
                OnLoadCompleted();
            });
        }

        public ActionResult<PedestrianProfileRequestModel> InitRequest(DriverModel driver)
        {
            Func<DriverModel, Logic.PedestrianProfileRequestLogic> initRequestDelegate = InitRequestDelegate;
            if (initRequestDelegate != null)
            {
                Logic.PedestrianProfileRequestLogic logic = initRequestDelegate(driver);
                return logic == null ? ActionResult<PedestrianProfileRequestModel>.GetErrorResult(new KeyNotFoundException()) : ActionResult<PedestrianProfileRequestModel>.GetValidResult(logic.Model);
            }
            else
                return ActionResult<PedestrianProfileRequestModel>.GetErrorResult(new NotSupportedException());
        }

        public ActionResult CallToDriver(DriverModel driverModel)
        {
            Func<DriverModel, ActionResult> callToDriverDelegate = CallToDriverDelegate;
            if (callToDriverDelegate == null)
                return callToDriverDelegate(driverModel);
            else
                return ActionResult.GetErrorResult(new NotSupportedException());
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

        protected virtual void OnSelectedDriverChanged()
        {
            EventHandler handler = SelectedDriverChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnLoadCompleted()
        {
            EventHandler handler = LoadCompleted;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnCheckCurrentRequestFailed()
        {
            EventHandler handler = CheckCurrentRequestFailed;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
