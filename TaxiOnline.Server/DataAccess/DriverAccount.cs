namespace TaxiOnline.Server.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DriverAccount
    {
        public int Id { get; set; }

        public Guid PersonId { get; set; }

        [StringLength(100)]
        public string PersonName { get; set; }

        [StringLength(50)]
        public string CarColor { get; set; }

        [StringLength(50)]
        public string CarBrand { get; set; }

        [StringLength(50)]
        public string CarNumber { get; set; }

        public virtual PersonAccount PersonAccount { get; set; }
    }
}
