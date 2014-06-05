namespace TaxiOnline.Server.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PersonAccount
    {
        public PersonAccount()
        {
            DriverAccounts = new HashSet<DriverAccount>();
            PedestrianAccounts = new HashSet<PedestrianAccount>();
            PersonsInfoes = new HashSet<PersonsInfo>();
        }

        public Guid Id { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        public string SkypeNumber { get; set; }

        public virtual ICollection<DriverAccount> DriverAccounts { get; set; }

        public virtual ICollection<PedestrianAccount> PedestrianAccounts { get; set; }

        public virtual ICollection<PersonsInfo> PersonsInfoes { get; set; }
    }
}
