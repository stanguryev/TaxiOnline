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
    internal class PedestrianProfileRequestLogic
    {
        private readonly PedestrianProfileRequestModel _model;
        private readonly PedestrianProfileLogic _user;
        private readonly AdaptersExtender _adaptersExtender;
        private readonly RequestDecorator _requestDecorator;

        public PedestrianProfileRequestModel Model
        {
            get { return _model; }
        } 

        internal PedestrianProfileRequestLogic(PedestrianProfileRequestModel model, AdaptersExtender adaptersExtender, PedestrianProfileLogic user)
        {
            _model = model;
            _adaptersExtender = adaptersExtender;
            _user = user;
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
                    _user.SetRequest(this);
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
            IPedestrianRequest requestSLO = _adaptersExtender.ServicesFactory.GetCurrentDataService().CreatePedestrianRequest(_user.Model.PersonId);
            requestSLO.TargetName = _model.Target.Name;
            requestSLO.TargetLocation = _model.Target.Location;
            return _adaptersExtender.ServicesFactory.GetCurrentDataService().PushPedestrianRequest(requestSLO);
        }

        private void CancelPendingCore()
        {
            _user.ResetPendigRequest(this);
        }

        private ActionResult CancelConfirmedCore()
        {
            ActionResult cancelResult = _adaptersExtender.ServicesFactory.GetCurrentDataService().RemovePedestrianRequest(_user.Model.PersonId);
            if (cancelResult.IsValid)
                _user.ResetConfirmedRequest(this);
            return cancelResult;
        }
    }
}
