using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Decorators;
using TaxiOnline.Logic.Models;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.Logic.Logic
{
    internal class DriverProfileResponseLogic
    {
        private readonly DriverProfileResponseModel _model;
        private readonly AdaptersExtender _adaptersExtender;
        private readonly PedestrianRequestLogic _request;
        private readonly DriverProfileLogic _responseAuthor;
        private readonly RequestDecorator _requestConfirmDecorator;
        private readonly RequestDecorator _requestRejectDecorator;

        public DriverProfileResponseModel Model
        {
            get { return _model; }
        }

        public DriverProfileResponseLogic(DriverProfileResponseModel model, AdaptersExtender adaptersExtender, PedestrianRequestLogic request, DriverProfileLogic responseAuthor)
        {
            _model = model;
            _adaptersExtender = adaptersExtender;
            _request = request;
            _responseAuthor = responseAuthor;
            _requestConfirmDecorator = new RequestDecorator(() => _model.ConfirmState, state => _model.ConfirmState = state, ConfirmCore, CancelPendingConfirmCore, CancelConfirmedCore);
            _requestRejectDecorator = new RequestDecorator(() => _model.RejectState, state => _model.RejectState = state, RejectCore, CancelPendingRejectCore, CancelRejectCore);
            model.ConfirmDelegate = Confirm;
            model.CancelConfirmDelegate = CancelConfirm;
            model.RejectDelegate = Reject;
            model.CancelRejectDelegate = CancelReject;
        }

        public void Confirm(Action<ActionResult> resultCallback)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                ActionResult confirmResult = _requestConfirmDecorator.Confirm();
                resultCallback(confirmResult);
                if (confirmResult.IsValid)
                    _responseAuthor.SetResponse(this);
            });
        }

        public void CancelConfirm(Action<ActionResult> resultCallback)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                ActionResult cancelResult = _requestConfirmDecorator.Cancel();
                resultCallback(cancelResult);
            });
        }

        public void Reject(Action<ActionResult> resultCallback)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                ActionResult rejectResult = _requestRejectDecorator.Confirm();
                resultCallback(rejectResult);
                if (rejectResult.IsValid)
                    _responseAuthor.SetResponse(this);
            });
        }

        public void CancelReject(Action<ActionResult> resultCallback)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                ActionResult cancelResult = _requestRejectDecorator.Cancel();
                resultCallback(cancelResult);
            });
        }

        private ActionResult ConfirmCore()
        {
            ActionResult<IDriverResponse> confirmResult = _adaptersExtender.ServicesFactory.GetCurrentDataService().ConfirmPedestrianRequest(_request.Model.RequestId);
            if (confirmResult.IsValid)
                _model.ResponseId = confirmResult.Result.Id;
            return confirmResult.IsValid ? ActionResult.ValidResult : ActionResult.GetErrorResult(confirmResult);
        }

        private void CancelPendingConfirmCore()
        {
            _responseAuthor.ResetPendigResponse(this);
        }

        private ActionResult CancelConfirmedCore()
        {
            return _adaptersExtender.ServicesFactory.GetCurrentDataService().RemoveDriverResponse(_model.ResponseId);
        }

        private ActionResult RejectCore()
        {
            return _adaptersExtender.ServicesFactory.GetCurrentDataService().RejectPedestrianRequest(_request.Model.RequestId);
        }

        private void CancelPendingRejectCore()
        {
            
        }

        private ActionResult CancelRejectCore()
        {
            return _adaptersExtender.ServicesFactory.GetCurrentDataService().StopRejectPedestrianRequest(_request.Model.RequestId);
        }
    }
}
