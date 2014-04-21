using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Models;

namespace TaxiOnline.Logic.Logic
{
    internal class PedestrianProfileLogic : ProfileLogic
    {
        private readonly PedestrianProfileModel _model;
        private readonly AdaptersExtender _adaptersExtender;
        private readonly InteractionLogic _interaction;

        public PedestrianProfileModel Model
        {
            get { return _model; }
        }

        public PedestrianProfileLogic(PedestrianProfileModel model, AdaptersExtender adaptersExtender, CityLogic city)
            : base(model, adaptersExtender, city)
        {
            _model = model;
            model.InitRequestDelegate = InitRequest;
        }

        public PedestrianProfileRequestLogic InitRequest()
        {
            PedestrianProfileRequestModel requestModel = new PedestrianProfileRequestModel(_model);
            _model.PendingRequest = requestModel;
            return new PedestrianProfileRequestLogic(requestModel, _adaptersExtender, this);
        }

        public void SetRequest(PedestrianProfileRequestLogic request)
        {
            _model.CurrentRequest = request.Model;
            _model.PendingRequest = null;
        }

        public void ResetPendigRequest(PedestrianProfileRequestLogic request)
        {
            _model.PendingRequest = null;
        }

        public void ResetConfirmedRequest(PedestrianProfileRequestLogic request)
        {
            _model.CurrentRequest = null;
        }
    }
}
