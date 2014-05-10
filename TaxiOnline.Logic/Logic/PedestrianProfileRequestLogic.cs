using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Decorators;
using TaxiOnline.Logic.Models;
using TaxiOnline.Toolkit.Collections.Helpers;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.CollectionsDecorators;

namespace TaxiOnline.Logic.Logic
{
    internal class PedestrianProfileRequestLogic
    {
        private readonly PedestrianProfileRequestModel _model;
        private readonly PedestrianProfileLogic _user;
        private readonly AdaptersExtender _adaptersExtender;
        private readonly RequestDecorator _requestDecorator;
        private DriverResponseLogic _response;
        //private UpdatableCollectionLoadDecorator<DriverResponseLogic, IDriverResponse> _availableResponses;

        public PedestrianProfileRequestModel Model
        {
            get { return _model; }
        }

        public DriverResponseLogic Response
        {
            get { return _response; }
            set { _response = value; }
        }

        internal PedestrianProfileRequestLogic(PedestrianProfileRequestModel model, AdaptersExtender adaptersExtender, PedestrianProfileLogic user)
        {
            _model = model;
            _adaptersExtender = adaptersExtender;
            _user = user;
            //_availableResponses = new UpdatableCollectionLoadDecorator<DriverResponseLogic, IDriverResponse>(RetriveAvailableResponses, CompareDriverResponseInfo, ValidateDriverResponseInfo, CreateDriverResponseLogic);
            _requestDecorator = new RequestDecorator(() => _model.State, state => _model.State = state, ConfirmCore, CancelPendingCore, CancelConfirmedCore);
            model.ConfirmDelegate = Confirm;
            model.CancelDelegate = Cancel;
            model.CallToDriverDelegate = CallToDriver;
            //model.EnumerateAvailableResponsesDelegate = EnumerateAvailableResponses;
            //_availableResponses.ItemsCollectionChanged += AvailableResponses_ItemsCollectionChanged;
        }

        private ActionResult CallToDriver()
        {
            return _user.Model.CallToDriver(_response.ResponseAuthor.Model);
        }

        //private ActionResult<IEnumerable<DriverResponseLogic>> EnumerateAvailableResponses()
        //{
        //    if (_availableResponses.Items == null)
        //        _availableResponses.FillItemsList();
        //    return ActionResult<IEnumerable<DriverResponseLogic>>.GetValidResult(_availableResponses.Items);
        //}

        public void Confirm(Action<ActionResult> resultCallback)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {                
                ActionResult confirmResult = _requestDecorator.Confirm();
                resultCallback(confirmResult);
                //if (confirmResult.IsValid)
                //    _user.SetRequest(this);
            });
        }

        public void Cancel(Action<ActionResult> resultCallback)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                ActionResult cancelResult = _requestDecorator.Cancel();
                resultCallback(cancelResult);
            });
        }

        private ActionResult ConfirmCore()
        {
            IPedestrianRequest requestSLO = _adaptersExtender.ServicesFactory.GetCurrentDataService().CreatePedestrianRequest(_user.Model.PersonId, _response.ResponseAuthor.Model.PersonId);
            requestSLO.TargetName = _model.Target.Name;
            requestSLO.TargetLocation = _model.Target.Location;
            requestSLO.Comment = _model.Comment;
            return _adaptersExtender.ServicesFactory.GetCurrentDataService().PushPedestrianRequest(requestSLO);
        }

        private void CancelPendingCore()
        {
            _user.Model.ModifyRequestsCollection(col => col.Remove(_model));
        }

        private ActionResult CancelConfirmedCore()
        {
            ActionResult cancelResult = _adaptersExtender.ServicesFactory.GetCurrentDataService().RemovePedestrianRequest(_user.Model.PersonId);
            if (cancelResult.IsValid)
                _user.Model.ModifyRequestsCollection(col => col.Remove(_model));
            return cancelResult;
        }

        //private ActionResult<IEnumerable<DriverResponseLogic>> RetriveAvailableResponses()
        //{
        //    ActionResult<IEnumerable<IDriverResponse>> requestResult = _adaptersExtender.ServicesFactory.GetCurrentDataService().EnumerateDriverResponses(_user.City.Model.Id);
        //    return requestResult.IsValid ? ActionResult<IEnumerable<DriverResponseLogic>>.GetValidResult(requestResult.Result.Select(slo => CreateDriverResponseLogic(slo))) : ActionResult<IEnumerable<DriverResponseLogic>>.GetErrorResult(requestResult);
        //}

        //private bool CompareDriverResponseInfo(DriverResponseLogic logic, IDriverResponse slo)
        //{
        //    return logic.Model.ResponseId == slo.Id;
        //}

        //private bool ValidateDriverResponseInfo(IDriverResponse slo)
        //{
        //    return slo.RequestId == _model.RequestId && !slo.IsCanceled;
        //}

        //private DriverResponseLogic CreateDriverResponseLogic(IDriverResponse slo)
        //{
        //    return new DriverResponseLogic(new DriverResponseModel(_model, _requestedUser.Model), _adaptersExtender, this, _requestedUser);
        //}

        //private void AvailableResponses_ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    _model.ModifyDriverResponsesCollection(col => ObservableCollectionHelper.ApplyChangesByObjects<DriverResponseLogic, DriverResponseModel>(e, col, l => l.Model, l => l.Model));
        //}
    }
}
