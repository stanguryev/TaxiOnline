using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.Exceptions;
using TaxiOnline.ClientInfrastructure.Exceptions.Enums;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Models;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.CollectionsDecorators;

namespace TaxiOnline.Logic.Logic
{
    internal class CityLogic
    {
        private readonly CityModel _model;
        private readonly AdaptersExtender _adaptersExtender;
        private readonly InteractionLogic _interaction;
        private readonly UpdatableCollectionLoadDecorator<PersonLogic, IPersonInfo> _persons;

        public CityModel Model
        {
            get { return _model; }
        }

        public CityLogic(CityModel model, AdaptersExtender adaptersExtender, InteractionLogic interaction)
        {
            _model = model;
            _adaptersExtender = adaptersExtender;
            _interaction = interaction;
            _persons = new UpdatableCollectionLoadDecorator<PersonLogic, IPersonInfo>(RetriveAllPersons, ComparePersonsInfo, p => p.IsOnline, CreatePersonsLogic);
            model.AuthenticateDelegate = Authenticate;
            model.EnumeratePersonsDelegate = EnumeratePersons;
        }

        public ActionResult<ProfileLogic> Authenticate(AuthenticationRequestModel requestModel)
        {
            requestModel.DeviceId = _adaptersExtender.ServicesFactory.GetCurrentHardwareService().GetDeviceId();
            ActionResult<MapPoint> locationResult = _adaptersExtender.ServicesFactory.GetCurrentHardwareService().GetCurrentLocation();
            if (!locationResult.IsValid)
                return ActionResult<ProfileLogic>.GetErrorResult(new HardwareServiceException(HardwareServiceErrors.NoLocationService));
            requestModel.CurrentLocation = locationResult.Result;
            AuthenticationRequestLogic authenticationRequestLogic = AuthenticationRequestLogic.Create(requestModel, _adaptersExtender, this);
            ActionResult<ProfileLogic> result = authenticationRequestLogic.Authenticate();
            if (result.IsValid)
                _interaction.CurrentProfile = result.Result;
            return result;
        }

        private ActionResult<IEnumerable<PersonLogic>> EnumeratePersons()
        {
            if (_persons.Items == null)
                _persons.FillItemsList();
            return ActionResult<IEnumerable<PersonLogic>>.GetValidResult(_persons.Items);
        }

        private ActionResult<IEnumerable<PersonLogic>> RetriveAllPersons()
        {
            ActionResult<IEnumerable<IPersonInfo>> requestResult = _adaptersExtender.ServicesFactory.GetCurrentDataService().EnumerateAllPersons(_model.Id);
            return requestResult.IsValid ? ActionResult<IEnumerable<PersonLogic>>.GetValidResult(requestResult.Result.Select(r => CreatePersonsLogic(r)).ToArray())
                : ActionResult<IEnumerable<PersonLogic>>.GetErrorResult(requestResult);
        }

        private bool ComparePersonsInfo(PersonLogic logic, IPersonInfo slo)
        {
            return logic.PersonModel.PersonId == slo.PersonId;
        }

        private PersonLogic CreatePersonsLogic(IPersonInfo personSLO)
        {
            if (personSLO is IPedestrianInfo)
                return new PedestrianLogic(new PedestrianModel((IPedestrianInfo)personSLO)
                {
                    PersonId = personSLO.PersonId
                }, _adaptersExtender, this);
            if (personSLO is IDriverInfo)
                return new DriverLogic(new DriverModel((IDriverInfo)personSLO)
                {
                    PersonId = personSLO.PersonId
                }, _adaptersExtender, this);
            else
                throw new NotImplementedException();
        }
    }
}
