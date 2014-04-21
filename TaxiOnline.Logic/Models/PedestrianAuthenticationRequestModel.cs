using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.Logic.Models
{
    public class PedestrianAuthenticationRequestModel : AuthenticationRequestModel
    {
        public override ParticipantTypes ParticipantType
        {
            get { return ParticipantTypes.Pedestrian; }
        }

        internal PedestrianAuthenticationRequestModel()
        {

        }        
    }
}
