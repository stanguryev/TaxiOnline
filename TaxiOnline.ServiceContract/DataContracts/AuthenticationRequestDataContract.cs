using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TaxiOnline.ServiceContract.DataContracts
{
    [DataContract]
    public class AuthenticationRequestDataContract
    {
        [DataMember]
        public string DeviceId { get; set; }

        [DataMember]
        public Guid CityId { get; set; }

        [DataMember]
        public double CurrentLocationLatidude { get; set; }

        [DataMember]
        public double CurrentLocationLongitude { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string SkypeNumber { get; set; }
    }
}
