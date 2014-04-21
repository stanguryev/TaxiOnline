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
    internal class DriverAuthenticationRequestLogic : AuthenticationRequestLogic
    {
        private readonly DriverAuthenticationRequestModel _model;

        public override ParticipantTypes ParticipantType
        {
            get { return ParticipantTypes.Driver; }
        }

        public DriverAuthenticationRequestLogic(DriverAuthenticationRequestModel model, AdaptersExtender adaptersExtender, CityLogic city)
            : base(model, adaptersExtender, city)
        {
            _model = model;
        }

        protected override ProfileLogic CreateProfileLogic(IPersonInfo personInfo)
        {
            IDriverInfo driverInfo = (IDriverInfo)personInfo;
            return new DriverProfileLogic(new DriverProfileModel
            {
                SkypeNumber = personInfo.SkypeNumber,
                PhoneNumber = personInfo.PhoneNumber,
                CarColor = driverInfo.PhoneNumber
            }, _adaptersExtender, _city);
        }

        protected override void FillRequest(IAuthenticationRequest request)
        {
            IDriverAuthenticationRequest driverRequest = (IDriverAuthenticationRequest)request;
            driverRequest.PhoneNumber = _model.PhoneNumber;
            driverRequest.SkypeNumber = _model.SkypeNumber;
            driverRequest.CarColor = _model.CarColor;
        }
    }
}
