using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Common.Enums;
using TaxiOnline.Logic.Models;
using TaxiOnline.Toolkit.Collections.Helpers;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.CollectionsDecorators;

namespace TaxiOnline.Logic.Logic
{
    internal class DriverProfileLogic : ProfileLogic
    {
        private readonly DriverProfileModel _model;
        private readonly UpdatableCollectionLoadDecorator<PedestrianLogic, IPedestrianInfo> _pedestrians;
        private readonly UpdatableCollectionLoadDecorator<PedestrianRequestLogic, IPedestrianRequest> _pedestrianRequests;
        private readonly UpdatableCollectionLoadDecorator<DriverProfileResponseLogic, IDriverResponse> _currentResponses;

        public DriverProfileModel Model
        {
            get { return _model; }
        }

        public SimpleCollectionLoadDecorator<PedestrianLogic> Pedestrians
        {
            get { return _pedestrians; }
        }

        public UpdatableCollectionLoadDecorator<PedestrianRequestLogic, IPedestrianRequest> PedestrianRequests
        {
            get { return _pedestrianRequests; }
        }

        public event NotifyCollectionChangedEventHandler PedestrianRequestsCollectionChanged
        {
            add { _pedestrianRequests.ItemsCollectionChanged += value; }
            remove { _pedestrianRequests.ItemsCollectionChanged -= value; }
        }

        public event NotifyCollectionChangedEventHandler PedestrianCollectionChanged
        {
            add { _pedestrians.ItemsCollectionChanged += value; }
            remove { _pedestrians.ItemsCollectionChanged -= value; }
        }

        internal DriverProfileLogic(DriverProfileModel model, AdaptersExtender adaptersExtender, CityLogic city)
            : base(model, adaptersExtender, city)
        {
            _model = model;
            model.InitResponseDelegate = InitResponse;
            model.EnumeratePedestrianRequestsDelegate = EnumeratePedestrianRequests;
            model.EnumeratePedestriansDelegate = EnumeratePedestrians;
            model.EnumerateCurrentResponsesDelegate = EnumerateCurrentResponses;
            _pedestrians = new UpdatableCollectionLoadDecorator<PedestrianLogic, IPedestrianInfo>(RetrivePedestrians, ComparePedestriansInfo, p => p.IsOnline, CreatePedestrianLogic);
            _pedestrianRequests = new UpdatableCollectionLoadDecorator<PedestrianRequestLogic, IPedestrianRequest>(RetrivePedestrianRequests, ComparePedestrianRequests, ValidatePedestrianRequest, CreatePedestrianRequestLogic);
            _currentResponses = new UpdatableCollectionLoadDecorator<DriverProfileResponseLogic, IDriverResponse>(RetriveDriverResponses, CompareDriverResponses, ValidateDriverResponse, CreateDriverResponseLogic);
            _adaptersExtender.ServicesFactory.GetCurrentDataService().PedestrianRequestChanged += DataService_PedestrianRequestChanged;
            _adaptersExtender.ServicesFactory.GetCurrentDataService().PedestrianInfoChanged += DataService_PedestrianInfoChanged;
            _pedestrians.ItemsCollectionChanged += Pedestrians_ItemsCollectionChanged;
            _pedestrians.RequestFailed += Pedestrians_RequestFailed;
            _pedestrianRequests.ItemsCollectionChanged += PedestrianRequests_ItemsCollectionChanged;
            _pedestrianRequests.RequestFailed += PedestrianRequests_RequestFailed;
        }

        public void SetResponse(DriverProfileResponseLogic response)
        {
            _model.AddCurrentResponse(response.Model);
            _model.RemovePendingResponse(response.Model);
        }

        public void ResetPendigResponse(DriverProfileResponseLogic response)
        {
            _model.RemovePendingResponse(response.Model);
        }

        public ActionResult<DriverProfileResponseLogic> InitResponse(Guid requestId)
        {
            PedestrianRequestLogic responseTarget = _pedestrianRequests.Items.FirstOrDefault(p => p.Model.RequestId == requestId);
            if (responseTarget != null)
            {
                DriverProfileResponseModel responseModel = new DriverProfileResponseModel(responseTarget.Model, _model);
                _model.AddPendingResponse(responseModel);
                return ActionResult<DriverProfileResponseLogic>.GetValidResult(new DriverProfileResponseLogic(responseModel, _adaptersExtender, responseTarget, this));
            }
            else
                return ActionResult<DriverProfileResponseLogic>.GetErrorResult(new KeyNotFoundException());
        }

        private ActionResult<IEnumerable<PedestrianRequestLogic>> EnumeratePedestrianRequests()
        {
            if (_pedestrianRequests.Items == null)
                _pedestrianRequests.FillItemsList();
            return ActionResult<IEnumerable<PedestrianRequestLogic>>.GetValidResult(_pedestrianRequests.Items);
        }

        private ActionResult<IEnumerable<PedestrianLogic>> EnumeratePedestrians()
        {
            if (_pedestrians.Items == null)
                _pedestrians.FillItemsList();
            return ActionResult<IEnumerable<PedestrianLogic>>.GetValidResult(_pedestrians.Items);
        }

        private ActionResult<IEnumerable<DriverProfileResponseLogic>> EnumerateCurrentResponses()
        {
            if (_currentResponses.Items == null)
                _currentResponses.FillItemsList();
            return ActionResult<IEnumerable<DriverProfileResponseLogic>>.GetValidResult(_currentResponses.Items);
        }

        private ActionResult<IEnumerable<PedestrianLogic>> RetrivePedestrians()
        {
            ActionResult<IEnumerable<IPedestrianInfo>> requestResult = _adaptersExtender.ServicesFactory.GetCurrentDataService().EnumeratePedestrians(_city.Model.Id);
            return requestResult.IsValid ? ActionResult<IEnumerable<PedestrianLogic>>.GetValidResult(requestResult.Result.Select(r => CreatePedestrianLogic(r)).ToArray())
                : ActionResult<IEnumerable<PedestrianLogic>>.GetErrorResult(requestResult);
        }

        private bool ComparePedestriansInfo(PedestrianLogic logic, IPedestrianInfo slo)
        {
            return logic.Model.PersonId == slo.PersonId;
        }

        private PedestrianLogic CreatePedestrianLogic(IPedestrianInfo personSLO)
        {
            return new PedestrianLogic(new PedestrianModel(personSLO)
            {
                PersonId = personSLO.PersonId,
                CurrentLocation = personSLO.CurrentLocation,
                PhoneNumber = personSLO.PhoneNumber,
                SkypeNumber = personSLO.SkypeNumber
            }, _adaptersExtender, _city);
        }

        private ActionResult<IEnumerable<PedestrianRequestLogic>> RetrivePedestrianRequests()
        {
            ActionResult<IEnumerable<IPedestrianRequest>> requestResult = _adaptersExtender.ServicesFactory.GetCurrentDataService().EnumeratePedestrianRequests(_city.Model.Id);
            return requestResult.IsValid ? ActionResult<IEnumerable<PedestrianRequestLogic>>.GetValidResult(requestResult.Result.Select(r => CreatePedestrianRequestLogic(r)).Where(l => l != null).ToArray())
                : ActionResult<IEnumerable<PedestrianRequestLogic>>.GetErrorResult(requestResult);
        }

        private bool ComparePedestrianRequests(PedestrianRequestLogic logic, IPedestrianRequest slo)
        {
            return logic.Model.RequestId == slo.Id;
        }

        private bool ValidatePedestrianRequest(IPedestrianRequest slo)
        {
            return !slo.IsCanceled;
        }

        private PedestrianRequestLogic CreatePedestrianRequestLogic(IPedestrianRequest requestSLO)
        {
            PedestrianLogic requestAuthor = _pedestrians.Items.FirstOrDefault(p => p.Model.PersonId == requestSLO.PedestrianId);
            return requestAuthor == null ? null : new PedestrianRequestLogic(new PedestrianRequestModel(requestAuthor.Model)
            {
                RequestId = requestSLO.Id,
                PaymentAmount = requestSLO.PaymentAmount,
                Currency = requestSLO.Currency,
                Comment = requestSLO.Comment,
                IsCancelled = requestSLO.IsCanceled,
            }, _adaptersExtender, requestAuthor, this);
        }

        private ActionResult<IEnumerable<DriverProfileResponseLogic>> RetriveDriverResponses()
        {
            ActionResult<IEnumerable<IDriverResponse>> requestResult = _city.EnumerateDriverResponses();
            return requestResult.IsValid ? ActionResult<IEnumerable<DriverProfileResponseLogic>>.GetValidResult(requestResult.Result.Select(r => CreateDriverResponseLogic(r)).Where(l => l != null).ToArray())
                : ActionResult<IEnumerable<DriverProfileResponseLogic>>.GetErrorResult(requestResult);
        }

        private bool CompareDriverResponses(DriverProfileResponseLogic logic, IDriverResponse slo)
        {
            return logic.Model.ResponseId == slo.Id;
        }

        private bool ValidateDriverResponse(IDriverResponse response)
        {
            return response.DriverId == _model.PersonId;
        }

        private DriverProfileResponseLogic CreateDriverResponseLogic(IDriverResponse responseSLO)
        {
            PedestrianRequestLogic request = _pedestrianRequests.Items.FirstOrDefault(r => r.Model.RequestId == responseSLO.RequestId);
            return request == null ? null : new DriverProfileResponseLogic(new DriverProfileResponseModel(request.Model, _model), _adaptersExtender, request, this);
        }

        private void DataService_PedestrianRequestChanged(object sender, ValueEventArgs<IPedestrianRequest> e)
        {
            _pedestrianRequests.Update(e.Value);
        }

        private void DataService_PedestrianInfoChanged(object sender, ValueEventArgs<IPedestrianInfo> e)
        {
            _pedestrians.Update(e.Value);
        }

        private void Pedestrians_ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _model.ModifyPedestriansCollection(col => ObservableCollectionHelper.ApplyChangesByObjects<PedestrianLogic, PedestrianModel>(e, col, l => l.Model, l => l.Model));
        }

        private void Pedestrians_RequestFailed(object sender, ActionResultEventArgs e)
        {
            _model.NotifyEnumratePedestriansFailed(e.Result);
        }

        private void PedestrianRequests_ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach(PedestrianRequestLogic request in e.NewItems.OfType<PedestrianRequestLogic>().ToArray())
                {
                    PedestrianLogic author = _pedestrians.Items.SingleOrDefault(p => p.Model.PersonId == request.Model.AuthorId);
                    if (author != null)
                        author.ResetCurrentRequest();
                }
            if (e.NewItems != null)
                foreach(PedestrianRequestLogic request in e.NewItems.OfType<PedestrianRequestLogic>().ToArray())
                {
                    PedestrianLogic author = _pedestrians.Items.SingleOrDefault(p => p.Model.PersonId == request.Model.AuthorId);
                    if (author != null)
                        author.SetCurrentRequest(request);
                }
            _model.ModifyPedestrianRequestsCollection(col => ObservableCollectionHelper.ApplyChangesByObjects<PedestrianRequestLogic, PedestrianRequestModel>(e, col, l => l.Model, l => l.Model));
        }

        private void PedestrianRequests_RequestFailed(object sender, ActionResultEventArgs e)
        {
            _model.NotifyEnumratePedestrianRequestsFailed(e.Result);
        }
    }
}
