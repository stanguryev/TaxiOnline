using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Models;
using TaxiOnline.Toolkit.Collections.Helpers;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.CollectionsDecorators;

namespace TaxiOnline.Logic.Logic
{
    internal class PedestrianProfileLogic : ProfileLogic
    {
        private readonly PedestrianProfileModel _model;
        private readonly AdaptersExtender _adaptersExtender;
        private readonly InteractionLogic _interaction;
        private readonly UpdatableCollectionLoadDecorator<DriverLogic, IDriverInfo> _drivers;

        public PedestrianProfileModel Model
        {
            get { return _model; }
        }

        public PedestrianProfileLogic(PedestrianProfileModel model, AdaptersExtender adaptersExtender, CityLogic city)
            : base(model, adaptersExtender, city)
        {
            _model = model;
            model.InitRequestDelegate = InitRequest;
            model.EnumerateDriversDelegate = EnumerateDrivers;
            model.CallToDriverDelegate = CallToDriver;
            _drivers = new UpdatableCollectionLoadDecorator<DriverLogic, IDriverInfo>(RetriveDrivers, CompareDriversInfo, p => p.IsOnline, CreateDriverLogic);
            _drivers.ItemsCollectionChanged += Drivers_ItemsCollectionChanged;
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

        private ActionResult CallToDriver(DriverModel driverModel)
        {
            string phoneNumber = driverModel.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.Cast<char>().Any(c => char.IsDigit(c)))
                return _adaptersExtender.ServicesFactory.GetCurrentHardwareService().PhoneCall(phoneNumber);
            else
                return ActionResult.GetErrorResult(new InvalidOperationException());
        }

        private ActionResult<IEnumerable<DriverLogic>> EnumerateDrivers()
        {
            if (_drivers.Items == null)
                _drivers.FillItemsList();
            return ActionResult<IEnumerable<DriverLogic>>.GetValidResult(_drivers.Items);
        }

        private ActionResult<IEnumerable<DriverLogic>> RetriveDrivers()
        {
            ActionResult<IEnumerable<IDriverInfo>> requestResult = _adaptersExtender.ServicesFactory.GetCurrentDataService().EnumerateDrivers(_city.Model.Id);
            return requestResult.IsValid ? ActionResult<IEnumerable<DriverLogic>>.GetValidResult(requestResult.Result.Select(r => CreateDriverLogic(r)).ToArray())
                : ActionResult<IEnumerable<DriverLogic>>.GetErrorResult(requestResult);
        }

        private bool CompareDriversInfo(DriverLogic logic, IDriverInfo slo)
        {
            return logic.Model.PersonId == slo.PersonId;
        }

        private DriverLogic CreateDriverLogic(IDriverInfo personSLO)
        {
            return new DriverLogic(new DriverModel()
            {
                PersonId = personSLO.PersonId
            }, _adaptersExtender, _city);
        }

        private void Drivers_ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _model.ModifyDriversCollection(col => ObservableCollectionHelper.ApplyChangesByObjects<DriverLogic, DriverModel>(e, col, l => l.Model, l => l.Model));
        }
    }
}
