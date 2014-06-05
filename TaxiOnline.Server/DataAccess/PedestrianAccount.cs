namespace TaxiOnline.Server.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PedestrianAccount
    {
        public int Id { get; set; }

        public Guid PersonId { get; set; }

        public virtual PersonAccount PersonAccount { get; set; }
    }
}
