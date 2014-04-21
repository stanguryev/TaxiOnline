using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            model.AuthenticateAsDriverDelegate = AuthenticateAsDriver;
            model.AuthenticateAsPedestrianDelegate = AuthenticateAsPedestrian;
            model.EnumeratePersonsDelegate = EnumeratePersons;
        }

        private ActionResult<IEnumerable<PersonLogic>> EnumeratePersons()
        {
            return ActionResult<IEnumerable<PersonLogic>>.GetValidResult(_persons.Items);
        }

        public ActionResult AuthenticateAsPedestrian(string deviceId)
        {
            ActionResult<IPedestrianInfo> info = _adaptersExtender.ServicesFactory.GetCurrentDataService().AuthenticateAsPedestrian(deviceId);
            if (info.IsValid)
            {
                PedestrianProfileModel profileModel = new PedestrianProfileModel
                {
                    Map = _interaction.Map.Model,
                    PersonId = info.Result.PersonId,
                    //PhoneNumber = info.Result.
                    //SkypeId = info.Result.
                };
                _interaction.CurrentProfile = new PedestrianProfileLogic(profileModel, _adaptersExtender, this);
                return ActionResult.ValidResult;
            }
            else
                return ActionResult.GetErrorResult(info);
        }

        public ActionResult AuthenticateAsDriver(string deviceId)
        {
            ActionResult<IDriverInfo> info = _adaptersExtender.ServicesFactory.GetCurrentDataService().AuthenticateAsDriver(deviceId);
            if (info.IsValid)
            {
                DriverProfileModel profileModel = new DriverProfileModel
                {
                    Map = _interaction.Map.Model,
                    PersonId = info.Result.PersonId,
                    //PhoneNumber = info.Result.
                    //SkypeId = info.Result.
                };
                _interaction.CurrentProfile = new DriverProfileLogic(profileModel, _adaptersExtender, this);
                return ActionResult.ValidResult;
            }
            else
                return ActionResult.GetErrorResult(info);
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
                return new PedestrianLogic(new PedestrianModel()
                {
                    PersonId = personSLO.PersonId
                }, _adaptersExtender, this);
            if (personSLO is IDriverInfo)
                return new DriverLogic(new DriverModel()
                {
                    PersonId = personSLO.PersonId
                }, _adaptersExtender, this);
            else
                throw new NotImplementedException();
        }
    }
}
