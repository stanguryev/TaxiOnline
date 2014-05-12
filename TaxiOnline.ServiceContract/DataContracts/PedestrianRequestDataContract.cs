using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TaxiOnline.ServiceContract.DataContracts
{
    [DataContract]
    public class PedestrianRequestDataContract
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid PedestrianId { get; set; }

        [DataMember]
        public Guid DriverId { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public bool IsCanceled { get; set; }
    }
}
