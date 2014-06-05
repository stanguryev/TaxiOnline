namespace TaxiOnline.Server.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PedestrianRequest
    {
        public PedestrianRequest()
        {
            DriverResponses = new HashSet<DriverResponse>();
        }

        public Guid Id { get; set; }

        public int Author { get; set; }

        public int Target { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }

        public virtual ICollection<DriverResponse> DriverResponses { get; set; }

        public virtual DriversInfo DriversInfo { get; set; }

        public virtual PedestriansInfo PedestriansInfo { get; set; }
    }
}
