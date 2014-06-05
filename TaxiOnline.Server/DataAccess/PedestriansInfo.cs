namespace TaxiOnline.Server.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PedestriansInfo")]
    public partial class PedestriansInfo
    {
        public PedestriansInfo()
        {
            PedestrianRequests = new HashSet<PedestrianRequest>();
        }

        public int Id { get; set; }

        public int PersonInfo { get; set; }

        public virtual ICollection<PedestrianRequest> PedestrianRequests { get; set; }

        public virtual PersonsInfo PersonsInfo { get; set; }
    }
}
