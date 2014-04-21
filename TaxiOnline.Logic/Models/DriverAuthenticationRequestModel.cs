using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.Logic.Models
{
    public class DriverAuthenticationRequestModel : AuthenticationRequestModel
    {
        private string _carColor;

        public override ParticipantTypes ParticipantType
        {
            get { return ParticipantTypes.Driver; }
        }

        public string CarColor
        {
            get { return _carColor; }
            set { _carColor = value; }
        }

        internal DriverAuthenticationRequestModel()
        {

        }
    }
}
