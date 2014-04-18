using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Models;

namespace TaxiOnline.Logic.Logic
{
    internal class CityLogic
    {
        private readonly CityModel _model;
        private readonly AdaptersExtender _adaptersExtender;
        private readonly InteractionLogic _interaction;

        public CityModel Model
        {
            get { return _model; }
        } 

        public CityLogic(CityModel model, AdaptersExtender adaptersExtender, InteractionLogic interaction)
        {
            _model = model;
            _adaptersExtender = adaptersExtender;
            _interaction = interaction;
        }
    }
}
