using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Models;

namespace TaxiOnline.Logic.Logic
{
    internal class PedestrianLogic
    {
        private readonly PedestrianModel _model;
        private readonly AdaptersExtender _adaptersExtender;
        private readonly InteractionLogic _interaction;

        public PedestrianModel Model
        {
            get { return _model; }
        } 

        public PedestrianLogic(PedestrianModel model, AdaptersExtender adaptersExtender, InteractionLogic interaction)
        {
            _model = model;
            _adaptersExtender = adaptersExtender;
            _interaction = interaction;
        }        
    }
}
