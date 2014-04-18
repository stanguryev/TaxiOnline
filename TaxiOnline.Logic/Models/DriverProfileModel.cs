using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        internal Func<Guid, ActionResult<Logic.DriverProfileResponseLogic>> InitResponseDelegate { get; set; }

        internal Func<ActionResult<IEnumerable<Logic.PedestrianRequestLogic>>> EnumeratePedestrianRequestsDelegate { get; set; }

        internal Func<ActionResult<IEnumerable<Logic.PedestrianLogic>>> EnumeratePedestriansDelegate { get; set; }

        internal Func<ActionResult<IEnumerable<Logic.DriverProfileResponseLogic>>> EnumerateCurrentResponsesDelegate { get; set; }

        public DriverProfileModel()
        {
            _pedestrians = new SimpleCollectionLoadDecorator<PedestrianModel>(EnumeratePedestrians);
            _pedestrianRequests = new SimpleCollectionLoadDecorator<PedestrianRequestModel>(EnumeratePedestrianRequests);
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

        
    }
}
