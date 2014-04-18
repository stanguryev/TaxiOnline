using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TaxiOnline.ServiceContract.DataContracts
{
    [DataContract]
    public class CityDataContract
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public double InitialLatitude { get; set; }

        [DataMember]
        public double InitialLongitude { get; set; }

        [DataMember]
        public double InitialZoom { get; set; }
    }
}
