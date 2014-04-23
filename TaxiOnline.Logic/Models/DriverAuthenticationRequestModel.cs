using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.Logic.Models
{
    public class DriverAuthenticationRequestModel : AuthenticationRequestModel
    {
        private string _personName;
        private string _carColor;
        private string _carBrand;
        private string _carNumber;

        public override ParticipantTypes ParticipantType
        {
            get { return ParticipantTypes.Driver; }
        }

        public string PersonName
        {
            get { return _personName; }
            set { _personName = value; }
        }

        public string CarColor
        {
            get { return _carColor; }
            set { _carColor = value; }
        }

        public string CarBrand
        {
            get { return _carBrand; }
            set { _carBrand = value; }
        }

        public string CarNumber
        {
            get { return _carNumber; }
            set { _carNumber = value; }
        }

        internal DriverAuthenticationRequestModel()
        {

        }
    }
}
