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
    internal class PedestrianRequestLogic
    {
        private readonly PedestrianRequestModel _model;
        private readonly PedestrianLogic _requestAuthor;
        private readonly AdaptersExtender _adaptersExtender;
        private readonly DriverProfileLogic _requestTarget;

        public PedestrianRequestModel Model
        {
            get { return _model; }
        }

        public PedestrianRequestLogic(PedestrianRequestModel model, AdaptersExtender adaptersExtender, PedestrianLogic requestAuthor, DriverProfileLogic requestTarget)
        {
            _model = model;
            _adaptersExtender = adaptersExtender;
            _requestAuthor = requestAuthor;
            _requestTarget = requestTarget;
            model.InitResponseDelegate = InitResponse;
        }

        private ActionResult<DriverProfileResponseLogic> InitResponse()
        {
            return _requestTarget.InitResponse(_model.RequestId);
        }
    }
}
