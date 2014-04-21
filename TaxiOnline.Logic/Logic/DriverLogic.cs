using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Models;
using TaxiOnline.Toolkit.Collections.Helpers;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.Logic.Logic
{
    internal class DriverLogic : PersonLogic
    {
        private readonly DriverModel _model;

        public DriverModel Model
        {
            get { return _model; }
        }

        public DriverLogic(DriverModel model, AdaptersExtender adaptersExtender, CityLogic city)
            : base(model, adaptersExtender, city)
        {
            _model = model;
        }
    }
}
