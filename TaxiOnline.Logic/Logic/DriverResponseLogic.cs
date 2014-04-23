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
        private readonly PedestrianProfileRequestLogic _request;
        private readonly DriverLogic _responseAuthor;

        public DriverResponseModel Model
        {
            get { return _model; }
        }

        public DriverResponseLogic(DriverResponseModel model, AdaptersExtender adaptersExtender, PedestrianProfileRequestLogic request, DriverLogic responseAuthor)
        {
            _model = model;
            _adaptersExtender = adaptersExtender;
            _request = request;
            _responseAuthor = responseAuthor;
        }
    }
}
