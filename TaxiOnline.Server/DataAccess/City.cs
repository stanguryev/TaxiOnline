namespace TaxiOnline.Server.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class City
    {
        public City()
        {
            CityNames = new HashSet<CityName>();
            //OnlineDrivers = new HashSet<OnlineDriver>();
            //OnlinePedestrians = new HashSet<OnlinePedestrian>();
            PersonsInfo = new HashSet<PersonsInfo>();
        }

        public Guid Id { get; set; }

        public double InitialLatitude { get; set; }

        public double InitialLongitude { get; set; }

        public double InitialZoom { get; set; }

        [StringLength(300)]
        public string PhoneConstraintPattern { get; set; }

        [StringLength(300)]
        public string PhoneCorrectionPattern { get; set; }

        public virtual ICollection<CityName> CityNames { get; set; }

        //public virtual ICollection<OnlineDriver> OnlineDrivers { get; set; }

        //public virtual ICollection<OnlinePedestrian> OnlinePedestrians { get; set; }

        public virtual ICollection<PersonsInfo> PersonsInfo { get; set; }
    }
}
