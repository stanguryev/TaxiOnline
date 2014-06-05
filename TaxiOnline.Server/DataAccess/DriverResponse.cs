namespace TaxiOnline.Server.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DriverResponses")]
    public partial class DriverResponse
    {
        public Guid Id { get; set; }

        public Guid RequestId { get; set; }

        public bool? IsAccepted { get; set; }

        public virtual PedestrianRequest PedestrianRequest { get; set; }
    }
}
