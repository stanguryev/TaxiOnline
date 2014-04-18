using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Decorators;
using TaxiOnline.Logic.Models;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.Logic.Logic
{
    internal class DriverResponseLogic
    {
        private readonly DriverResponseModel _model;
        private readonly AdaptersExtender _adaptersExtender;
        private readonly PedestrianRequestLogic _request;
        private readonly DriverProfileLogic _responseAuthor;

        public DriverResponseModel Model
        {
            get { return _model; }
        }

        public DriverResponseLogic(DriverResponseModel model, AdaptersExtender adaptersExtender, PedestrianRequestLogic request, DriverProfileLogic responseAuthor)
        {
            _model = model;
            _adaptersExtender = adaptersExtender;
            _request = request;
            _responseAuthor = responseAuthor;
        }
    }
}
