using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.Logic.Helpers;
using TaxiOnline.Toolkit.Collections.Special;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.CollectionsDecorators;

namespace TaxiOnline.Logic.Models
{
    public class DriverProfileModel : ProfileModel
    {
        private readonly ReadonlyCollectionDecorator<DriverProfileResponseModel> _pendingResponses = new ReadonlyCollectionDecorator<DriverProfileResponseModel>();
        private readonly SimpleCollectionLoadDecorator<DriverProfileResponseModel> _currentResponses;
        private readonly SimpleCollectionLoadDecorator<PedestrianRequestModel> _pedestrianRequests;
        private readonly SimpleCollectionLoadDecorator<PedestrianModel> _pedestrians;
        private PedestrianRequestModel _selectedPedestrianRequest;
        private string _personName;
        private string _carColor;
        private string _carBrand;
        private string _carNumber;

        public IEnumerable<DriverProfileResponseModel> PendingResponses
        {
            get { return _pendingResponses.Items; }
        }

        public IEnumerable<DriverProfileResponseModel> CurrentResponses
        {
            get { return _currentResponses.Items; }
        }

        public IEnumerable<PedestrianModel> Pedestrians
        {
            get { return _pedestrians.Items; }
        }

        public IEnumerable<PedestrianRequestModel> PedestrianRequests
        {
            get { return _pedestrianRequests.Items; }
        }

        public PedestrianRequestModel SelectedPedestrianRequest
        {
            get { return _selectedPedestrianRequest; }
            set { _selectedPedestrianRequest = value; }
        }

        public string PersonName
        {
            get { return _personName; }
            set { _personName = value; }
        }

        public string CarColor
        {
            get { return _carColor; }
            set { _carColor = value; }
        }

        public string CarBrand
        {
            get { return _carBrand; }
            set { _carBrand = value; }
        }

        public string CarNumber
        {
            get { return _carNumber; }
            set { _carNumber = value; }
        }

        public event EventHandler LoadCompleted;

        public event EventHandler PedestriansChanged
        {
            add { _pedestrians.ItemsChanged += value; }
            remove { _pedestrians.ItemsChanged -= value; }
        }

        public event NotifyCollectionChangedEventHandler PedestriansCollectionChanged
        {
            add { _pedestrians.ItemsCollectionChanged += value; }
            remove { _pedestrians.ItemsCollectionChanged -= value; }
        }

        public event EventHandler PedestrianRequestsChanged
        {
            add { _pedestrianRequests.ItemsChanged += value; }
            remove { _pedestrianRequests.ItemsChanged -= value; }
        }

        public event NotifyCollectionChangedEventHandler PedestrianRequestsCollectionChanged
        {
            add { _pedestrianRequests.ItemsCollectionChanged += value; }
            remove { _pedestrianRequests.ItemsCollectionChanged -= value; }
        }

        public event ActionResultEventHandler EnumratePedestriansFailed;

        public event ActionResultEventHandler EnumratePedestrianRequestsFailed;

        internal Func<Guid, ActionResult<Logic.DriverProfileResponseLogic>> InitResponseDelegate { get; set; }

        internal Func<ActionResult<IEnumerable<Logic.PedestrianRequestLogic>>> EnumeratePedestrianRequestsDelegate { get; set; }

        internal Func<ActionResult<IEnumerable<Logic.PedestrianLogic>>> EnumeratePedestriansDelegate { get; set; }

        internal Func<ActionResult<IEnumerable<Logic.DriverProfileResponseLogic>>> EnumerateCurrentResponsesDelegate { get; set; }

        public DriverProfileModel(MapModel map)
            : base(map)
        {
            _pedestrians = new SimpleCollectionLoadDecorator<PedestrianModel>(EnumeratePedestrians);
            _pedestrianRequests = new SimpleCollectionLoadDecorator<PedestrianRequestModel>(EnumeratePedestrianRequests);
        }

        public void BeginLoad()
        {
            Task.Factory.StartNew(() =>
            {
                _pedestrians.FillItemsList();
                _pedestrianRequests.FillItemsList();
                OnLoadCompleted();
            });
        }

        public ActionResult InitResponse(PedestrianRequestModel request)
        {
            Func<Guid, ActionResult<Logic.DriverProfileResponseLogic>> initResponseDelegate = InitResponseDelegate;
            if (initResponseDelegate != null)
            {
                ActionResult<Logic.DriverProfileResponseLogic> initResult = initResponseDelegate(request.RequestId);
                return initResult.IsValid ? ActionResult.ValidResult : ActionResult.GetErrorResult(initResult);
            }
            else
                return ActionResult.GetErrorResult(new NotSupportedException());
        }

        internal void ModifyPedestriansCollection(Action<IList<PedestrianModel>> modificationDelegate)
        {
            _pedestrians.ModifyCollection(modificationDelegate);
        }

        internal void NotifyEnumratePedestriansFailed(ActionResult actionResult)
        {
            OnEnumratePedestriansFailed(actionResult);
        }

        internal void ModifyPedestrianRequestsCollection(Action<IList<PedestrianRequestModel>> modificationDelegate)
        {
            _pedestrianRequests.ModifyCollection(modificationDelegate);
        }

        internal void NotifyEnumratePedestrianRequestsFailed(ActionResult actionResult)
        {
            OnEnumratePedestrianRequestsFailed(actionResult);
        }

        internal void AddCurrentResponse(DriverProfileResponseModel response)
        {
            _currentResponses.ModifyCollection(col => col.Add(response));
        }

        internal void RemoveCurrentResponse(DriverProfileResponseModel response)
        {
            _currentResponses.ModifyCollection(col => col.Remove(response));
        }

        internal void AddPendingResponse(DriverProfileResponseModel response)
        {
            _pendingResponses.ModifyCollection(col => col.Add(response));
        }

        internal void RemovePendingResponse(DriverProfileResponseModel response)
        {
            _pendingResponses.ModifyCollection(col => col.Remove(response));
        }

        private ActionResult<IEnumerable<PedestrianRequestModel>> EnumeratePedestrianRequests()
        {
            return UpdateHelper.EnumerateModels(EnumeratePedestrianRequestsDelegate, l => l.Model);
        }

        private ActionResult<IEnumerable<PedestrianModel>> EnumeratePedestrians()
        {
            return UpdateHelper.EnumerateModels(EnumeratePedestriansDelegate, l => l.Model);
        }

        private ActionResult<IEnumerable<DriverProfileResponseModel>> EnumerateCurrentResponses()
        {
            return UpdateHelper.EnumerateModels(EnumerateCurrentResponsesDelegate, l => l.Model);
        }

        protected virtual void OnLoadCompleted()
        {
            EventHandler handler = LoadCompleted;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnEnumratePedestriansFailed(ActionResult errorResult)
        {
            ActionResultEventHandler handler = EnumratePedestriansFailed;
            if (handler != null)
                handler(this, new ActionResultEventArgs(errorResult));
        }

        protected virtual void OnEnumratePedestrianRequestsFailed(ActionResult errorResult)
        {
            ActionResultEventHandler handler = EnumratePedestrianRequestsFailed;
            if (handler != null)
                handler(this, new ActionResultEventArgs(errorResult));
        }
    }
}
