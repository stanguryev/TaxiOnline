namespace TaxiOnline.Server.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PersonsInfo")]
    public partial class PersonsInfo
    {
        public PersonsInfo()
        {
            DriversInfoes = new HashSet<DriversInfo>();
            PedestriansInfoes = new HashSet<PedestriansInfo>();
        }

        public int Id { get; set; }

        public Guid PersonId { get; set; }

        public Guid CityId { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public double? Altitude { get; set; }

        public virtual City City { get; set; }

        public virtual ICollection<DriversInfo> DriversInfoes { get; set; }

        public virtual ICollection<PedestriansInfo> PedestriansInfoes { get; set; }

        public virtual PersonAccount PersonAccount { get; set; }
    }
}
