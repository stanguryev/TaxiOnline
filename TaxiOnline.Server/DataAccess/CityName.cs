namespace TaxiOnline.Server.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CityName
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(8)]
        public string CultureName { get; set; }

        public Guid CityId { get; set; }

        [StringLength(500)]
        public string PhoneConstraintDescription { get; set; }

        public virtual City City { get; set; }
    }
}
