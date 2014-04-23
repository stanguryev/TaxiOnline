using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Models;

namespace TaxiOnline.Logic.Logic
{
    internal class PedestrianLogic : PersonLogic
    {
        private readonly PedestrianModel _model;

        public PedestrianModel Model
        {
            get { return _model; }
        }

        public PedestrianLogic(PedestrianModel model, AdaptersExtender adaptersExtender, CityLogic city)
            : base(model, adaptersExtender, city)
        {
            _model = model;
        }

        public void SetCurrentRequest(PedestrianRequestLogic request)
        {
            _model.CurrentRequest = request.Model;
        }

        public void ResetCurrentRequest()
        {
            _model.CurrentRequest = null;
        }
    }
}
