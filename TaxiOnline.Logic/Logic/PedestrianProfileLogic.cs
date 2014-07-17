using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Models;
using TaxiOnline.Toolkit.Collections.Helpers;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.CollectionsDecorators;

namespace TaxiOnline.Logic.Logic
{
    internal class PedestrianProfileLogic : ProfileLogic
    {
        private readonly PedestrianProfileModel _model;
        private readonly UpdatableCollectionLoadDecorator<DriverLogic, IDriverInfo> _drivers;
        private readonly UpdatableCollectionLoadDecorator<PedestrianProfileRequestLogic, IPedestrianRequest> _requests;
        private readonly SimpleCollectionLoadDecorator<DriverResponseLogic> _acceptedResponses;

        public PedestrianProfileModel Model
        {
            get { return _model; }
        }

        public CityLogic City
        {
            get { return _city; }
        }

        public PedestrianProfileLogic(PedestrianProfileModel model, AdaptersExtender adaptersExtender, CityLogic city)
            : base(model, adaptersExtender, city)
        {
            _model = model;
            model.InitRequestDelegate = InitRequest;
            model.EnumerateDriversDelegate = EnumerateDrivers;
            model.EnumerateRequestsDelegate = EnumerateRequests;
            model.EnumerateAcceptedResponsesDelegate = EnumerateAcceptedResponses;
            model.CallToDriverDelegate = CallToDriver;
            _drivers = new UpdatableCollectionLoadDecorator<DriverLogic, IDriverInfo>(RetriveDrivers, CompareDriversInfo, p => p.IsOnline, CreateDriverLogic);
            _requests = new UpdatableCollectionLoadDecorator<PedestrianProfileRequestLogic, IPedestrianRequest>(RetriveRequests, CompareRequestsInfo, ValidateRequest, CreateRequestLogic);
            _acceptedResponses = new SimpleCollectionLoadDecorator<DriverResponseLogic>(RetriveAcceptedResponse);
            _adaptersExtender.ServicesFactory.GetCurrentDataService().DriverInfoChanged += DataService_DriverInfoChanged;
            _drivers.ItemsCollectionChanged += Drivers_ItemsCollectionChanged;
            _acceptedResponses.ItemsCollectionChanged += AcceptedResponses_ItemsCollectionChanged;
        }

        public PedestrianProfileRequestLogic InitRequest(DriverModel driver)
        {

            DriverLogic driverLogic = _drivers.Items.SingleOrDefault(d => d.Model == driver);
            if (driverLogic == null)
                return null;
            PedestrianProfileRequestModel requestModel = new PedestrianProfileRequestModel(_model, driverLogic.Model);
            _model.ModifyRequestsCollection(col => col.Add(requestModel));
            PedestrianProfileRequestLogic outResult = new PedestrianProfileRequestLogic(requestModel, _adaptersExtender, this);
            outResult.Response = new DriverResponseLogic(requestModel.Response, _adaptersExtender, outResult, driverLogic);
            return outResult;
        }

        //public void SetRequest(PedestrianProfileRequestLogic request)
        //{
        //    _model.CurrentRequest = request.Model;
        //    _model.PendingRequest = null;
        //}

        //public void ResetPendigRequest(PedestrianProfileRequestLogic request)
        //{
        //    _model.PendingRequest = null;
        //}

        //public void ResetConfirmedRequest(PedestrianProfileRequestLogic request)
        //{
        //    _model.CurrentRequest = null;
        //}

        private ActionResult CallToDriver(DriverModel driverModel)
        {
            string phoneNumber = driverModel.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.Cast<char>().Any(c => char.IsDigit(c)))
                return _adaptersExtender.ServicesFactory.GetCurrentHardwareService().PhoneCall(phoneNumber);
            else
                return ActionResult.GetErrorResult(new InvalidOperationException());
        }

        private ActionResult<IEnumerable<DriverLogic>> EnumerateDrivers()
        {
            if (_drivers.Items == null)
                _drivers.FillItemsList();
            return _drivers.Items == null ? ActionResult<IEnumerable<DriverLogic>>.GetErrorResult(new Exception()) : ActionResult<IEnumerable<DriverLogic>>.GetValidResult(_drivers.Items);
        }

        private ActionResult<IEnumerable<PedestrianProfileRequestLogic>> EnumerateRequests()
        {
            if (_requests.Items == null)
                _requests.FillItemsList();
            return _requests.Items == null ? ActionResult<IEnumerable<PedestrianProfileRequestLogic>>.GetErrorResult(new Exception()) : ActionResult<IEnumerable<PedestrianProfileRequestLogic>>.GetValidResult(_requests.Items);
        }

        private ActionResult<IEnumerable<DriverResponseLogic>> EnumerateAcceptedResponses()
        {
            if (_acceptedResponses.Items == null)
                _acceptedResponses.FillItemsList();
            if (_acceptedResponses.Items != null)
                foreach (DriverResponseLogic response in _acceptedResponses.Items)
                    response.ResponseAuthor.Model.HasAcceptedRequest = true;
            return _acceptedResponses.Items == null ? ActionResult<IEnumerable<DriverResponseLogic>>.GetErrorResult(new Exception()) : ActionResult<IEnumerable<DriverResponseLogic>>.GetValidResult(_acceptedResponses.Items);
        }

        private ActionResult<IEnumerable<DriverLogic>> RetriveDrivers()
        {
            ActionResult<IEnumerable<IDriverInfo>> requestResult = _adaptersExtender.ServicesFactory.GetCurrentDataService().EnumerateDrivers(_city.Model.Id);
            return requestResult.IsValid ? ActionResult<IEnumerable<DriverLogic>>.GetValidResult(requestResult.Result.Select(r => CreateDriverLogic(r)).ToArray())
                : ActionResult<IEnumerable<DriverLogic>>.GetErrorResult(requestResult);
        }

        private bool CompareDriversInfo(DriverLogic logic, IDriverInfo slo)
        {
            return logic.Model.PersonId == slo.PersonId;
        }

        private DriverLogic CreateDriverLogic(IDriverInfo personSLO)
        {
            return new DriverLogic(new DriverModel(personSLO)
            {
                PersonId = personSLO.PersonId
            }, _adaptersExtender, _city);
        }

        private ActionResult<IEnumerable<PedestrianProfileRequestLogic>> RetriveRequests()
        {
            ActionResult<IEnumerable<IPedestrianRequest>> requestResult = _adaptersExtender.ServicesFactory.GetCurrentDataService().EnumeratePedestrianRequests(_city.Model.Id);
            return requestResult.IsValid ? ActionResult<IEnumerable<PedestrianProfileRequestLogic>>.GetValidResult(requestResult.Result.Select(r => CreateRequestLogic(r)).Where(r => r != null).ToArray())
                : ActionResult<IEnumerable<PedestrianProfileRequestLogic>>.GetErrorResult(requestResult);
        }

        private bool CompareRequestsInfo(PedestrianProfileRequestLogic logic, IPedestrianRequest slo)
        {
            return logic.Model.RequestId == slo.Id;
        }

        private bool ValidateRequest(IPedestrianRequest requestSLO)
        {
            return requestSLO.PedestrianId == _model.PersonId && !requestSLO.IsCanceled;
        }

        private PedestrianProfileRequestLogic CreateRequestLogic(IPedestrianRequest requestSLO)
        {
            DriverLogic driver = _city.Persons.OfType<DriverLogic>().SingleOrDefault(d => d.Model.PersonId == requestSLO.DriverId);
            if (driver == null)
                return null;
            PedestrianProfileRequestLogic outResult = new PedestrianProfileRequestLogic(new PedestrianProfileRequestModel(_model, driver.Model)
            {
                RequestId = requestSLO.Id,
                Comment = requestSLO.Comment
            }, _adaptersExtender, this);
            outResult.Response = new DriverResponseLogic(new DriverResponseModel(outResult.Model, driver.Model), _adaptersExtender, outResult, driver);
            return outResult;
        }

        private ActionResult<IEnumerable<DriverResponseLogic>> RetriveAcceptedResponse()
        {
            ActionResult<IEnumerable<IDriverResponse>> requestResult = _city.EnumerateDriverResponses();
            if (!requestResult.IsValid)
                return ActionResult<IEnumerable<DriverResponseLogic>>.GetErrorResult(requestResult);
            IEnumerable<PedestrianProfileRequestLogic> requests = _requests.Items;
            if (requests == null)
                return ActionResult<IEnumerable<DriverResponseLogic>>.GetErrorResult(new KeyNotFoundException());
            IDriverResponse[] ownResponses = requestResult.Result.Where(r => requests.Any(req => req.Model.RequestId == r.RequestId)).ToArray();
            foreach (PedestrianProfileRequestLogic request in requests)
            {
                IDriverResponse response = ownResponses.FirstOrDefault(r => r.RequestId == request.Model.RequestId);
                if (response != null)
                    request.Response.Model.State = response.State;
            }
            return ActionResult<IEnumerable<DriverResponseLogic>>.GetValidResult(requests.Select(r => r.Response).Where(r => r.Model.State == ClientInfrastructure.Data.DriverResponseState.Accepted).ToArray());
        }

        private void Drivers_ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _model.ModifyDriversCollection(col => ObservableCollectionHelper.ApplyChangesByObjects<DriverLogic, DriverModel>(e, col, l => l.Model, l => l.Model));
        }

        private void AcceptedResponses_ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _model.ModifyAcceptedResponsesCollection(col => ObservableCollectionHelper.ApplyChangesByObjects<DriverResponseLogic, DriverResponseModel>(e, col, l => l.Model, l => l.Model));
            if (e.NewItems != null)
                foreach (DriverResponseLogic response in e.NewItems.OfType<DriverResponseLogic>().ToArray())
                    response.ResponseAuthor.Model.HasAcceptedRequest = true;
            if (e.OldItems != null)
                foreach (DriverResponseLogic response in e.OldItems.OfType<DriverResponseLogic>().ToArray())
                    response.ResponseAuthor.Model.HasAcceptedRequest = true;
        }

        private void DataService_DriverInfoChanged(object sender, ValueEventArgs<IDriverInfo> e)
        {
            _drivers.Update(e.Value);
        }
    }
}
