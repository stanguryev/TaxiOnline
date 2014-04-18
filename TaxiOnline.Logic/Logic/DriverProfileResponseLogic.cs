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
        private readonly RequestDecorator _requestDecorator;

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
            _requestDecorator = new RequestDecorator(() => _model.State, state => _model.State = state, ConfirmCore, CancelPendingCore, CancelConfirmedCore);
            model.ConfirmDelegate = Confirm;
            model.CancelDelegate = Cancel;
        }

        public void Confirm(Action<ActionResult> resultCallback)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                ActionResult confirmResult = _requestDecorator.Confirm();
                resultCallback(confirmResult);
                if (confirmResult.IsValid)
                    _responseAuthor.SetResponse(this);
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
            ActionResult<IDriverResponse> confirmResult = _adaptersExtender.ServicesFactory.GetCurrentDataService().ConfirmPedestrianRequest(_request.Model.RequestId, _responseAuthor.Model.PersonId);
            if (confirmResult.IsValid)
                _model.ResponseId = confirmResult.Result.Id;
            return confirmResult.IsValid ? ActionResult.ValidResult : ActionResult.GetErrorResult(confirmResult);
        }

        private void CancelPendingCore()
        {
            _responseAuthor.ResetPendigResponse(this);
        }

        private ActionResult CancelConfirmedCore()
        {
            return _adaptersExtender.ServicesFactory.GetCurrentDataService().RemoveDriverResponse(_model.ResponseId);
        }
    }
}
