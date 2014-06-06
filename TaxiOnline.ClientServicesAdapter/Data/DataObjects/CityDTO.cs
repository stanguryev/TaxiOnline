using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiOnline.ClientServicesAdapter.Data.DataObjects
{
    public class CityDTO
    {
        public Guid Id { get; set; }

        public double InitialLatitude { get; set; }

        public double InitialLongitude { get; set; }

        public double InitialZoom { get; set; }

        public string PhoneConstraintPattern { get; set; }

        public string PhoneCorrectionPattern { get; set; }

        public string CityName { get; set; }

        public string PhoneConstraintDescription { get; set; }
    }
}
