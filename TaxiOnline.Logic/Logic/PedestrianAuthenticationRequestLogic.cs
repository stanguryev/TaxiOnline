using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Models;

namespace TaxiOnline.Logic.Logic
{
    internal class PedestrianAuthenticationRequestLogic : AuthenticationRequestLogic
    {
        private readonly PedestrianAuthenticationRequestModel _model;

        public override ParticipantTypes ParticipantType
        {
            get { return ParticipantTypes.Pedestrian; }
        }

        public PedestrianAuthenticationRequestLogic(PedestrianAuthenticationRequestModel model, AdaptersExtender adaptersExtender, CityLogic city)
            : base(model, adaptersExtender, city)
        {
            _model = model;
        }

        protected override ProfileLogic CreateProfileLogic(IPersonInfo personInfo)
        {
            return new PedestrianProfileLogic(new PedestrianProfileModel(_city.Model.Map)
            {
                SkypeNumber = personInfo.SkypeNumber,
                PhoneNumber = personInfo.PhoneNumber
            }, _adaptersExtender, _city);
        }

        protected override void FillRequest(IAuthenticationRequest request)
        {
            IPedestrianAuthenticationRequest pedestrianRequest = (IPedestrianAuthenticationRequest)request;
            pedestrianRequest.PhoneNumber = _model.PhoneNumber;
            pedestrianRequest.SkypeNumber = _model.SkypeNumber;
            pedestrianRequest.CurrentLocation = _model.CurrentLocation;
        }
    }
}
