using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Models;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.Logic.Logic
{
    internal abstract class AuthenticationRequestLogic
    {
        private readonly AuthenticationRequestModel _model;
        protected readonly AdaptersExtender _adaptersExtender;
        protected readonly CityLogic _city;

        public abstract ParticipantTypes ParticipantType { get; }

        public AuthenticationRequestLogic(AuthenticationRequestModel model, AdaptersExtender adaptersExtender, CityLogic city)
        {
            _model = model;
            _adaptersExtender = adaptersExtender;
            _city = city;
        }

        public static AuthenticationRequestLogic Create(AuthenticationRequestModel model, AdaptersExtender adaptersExtender, CityLogic city)
        {
            switch (model.ParticipantType)
            {
                case ParticipantTypes.Driver:
                    return new DriverAuthenticationRequestLogic((DriverAuthenticationRequestModel)model, adaptersExtender, city);
                    break;
                case ParticipantTypes.Pedestrian:
                    return new PedestrianAuthenticationRequestLogic((PedestrianAuthenticationRequestModel)model, adaptersExtender, city);
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        public ActionResult<ProfileLogic> Authenticate()
        {
            IAuthenticationRequest request = _adaptersExtender.ServicesFactory.GetCurrentDataService().CreateAuthenticationRequest(ParticipantType, null, _city.Model.Id);
            FillRequest(request);
            ActionResult<IPersonInfo> result = _adaptersExtender.ServicesFactory.GetCurrentDataService().Authenticate(request);
            return result.IsValid ? ActionResult<ProfileLogic>.GetValidResult(CreateProfileLogic(result.Result)) : ActionResult<ProfileLogic>.GetErrorResult(result);
        }

        protected abstract ProfileLogic CreateProfileLogic(IPersonInfo personInfo);

        protected abstract void FillRequest(IAuthenticationRequest request);
    }
}
